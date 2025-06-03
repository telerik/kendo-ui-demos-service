using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using signalr_for_aspnet_core.Models;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using System.Runtime.CompilerServices;
using SharpToken;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace signalr_for_aspnet_core.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatClient _chatClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IWebHostEnvironment _env;
        private const int MaxConversationsPerUser = 300; // For testing purposes. Reduce to 3 in production.
        private const int MaxInputTokensPerConversation = 1000;
        private const int MaxOutputTokensPerConversation = 500;
        private const string DefaultSystemPrompt =
    @"You are an AI assistant designed to help users with various tasks and provide clear, accurate, and useful information. 
                Your goal is to understand the user's input, respond politely, and guide them toward the most appropriate and helpful solution. 
                Be concise, professional, and always remain neutral and respectful. 
                Avoid giving opinions or taking sides in discussions, and focus on providing valuable insights. 
                If you do not have enough information to respond accurately, ask the user for clarification.
                Use markdown for formatting when it is appropriate.";

        public ChatHub(IChatClient chatClient, IMemoryCache memoryCache, IWebHostEnvironment env)
        {
            _chatClient = chatClient;
            _memoryCache = memoryCache;
            _env = env;
        }

        private int CountTokens(string message)
        {
            var encoding = GptEncoding.GetEncoding("cl100k_base");
            return encoding.Encode(message).Count;
        }

        private TokenCount GetConversationTotalTokenCount(List<ChatMessage> messages)
        {
            var encoding = GptEncoding.GetEncoding("cl100k_base");
            int totalOutputTokens = 0;
            int totalInputTokens = 0;

            foreach (var message in messages)
            {
                if (message.Role == ChatRole.Assistant)
                {
                    totalOutputTokens += CountTokens(message.Text);
                }

                if (message.Role == ChatRole.User)
                {
                    totalInputTokens += CountTokens(message.Text);
                }
            }
            return new TokenCount() { InputTokens = totalInputTokens, OutputTokens = totalOutputTokens };
        }

        private UserChatHistory GetUserChatHistory(string userId)
        {
            var ipAddress = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString();
            return _memoryCache.GetOrCreate($"chat_{userId}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                entry.Size = 1;
                return new UserChatHistory { IpAddress = ipAddress };
            });
        }

        private bool CanStartNewConversation(string ipAddress)
        {
            var cacheKey = $"ip_convo_count_{ipAddress}";
            if (!_memoryCache.TryGetValue<int>(cacheKey, out var count))
            {
                count = 0;
            }
            if (count >= 3)
            {
                return false;
            }
            _memoryCache.Set(cacheKey, count + 1, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                Size = 1
            });
            return true;
        }

        private List<ChatMessage> GetCurrentConversation(string userId)
        {
            var userHistory = GetUserChatHistory(userId);
            userHistory.LastActivity = DateTime.UtcNow;

            if (userHistory.Conversations.Count == 0)
            {
                var newConversation = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.System, DefaultSystemPrompt)
                };
                userHistory.Conversations.Add(newConversation);
            }

            var currentConversation = userHistory.Conversations[userHistory.CurrentConversationIndex];

            return currentConversation;
        }

        private List<ChatMessage> StartNewConversation(string userId, UserChatHistory userHistory)
        {
            var newConversation = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, DefaultSystemPrompt)
            };

            if (userHistory.Conversations.Count >= MaxConversationsPerUser)
            {
                userHistory.Conversations.RemoveAt(0);
                userHistory.CurrentConversationIndex = userHistory.Conversations.Count;
            }
            else
            {
                userHistory.CurrentConversationIndex++;
            }

            userHistory.Conversations.Add(newConversation);

            _memoryCache.Set($"chat_{userId}", userHistory, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                SlidingExpiration = TimeSpan.FromMinutes(30),
                Size = 1
            });

            return newConversation;
        }

        public async IAsyncEnumerable<StreamedChatMessage> StreamMessage(
            string message,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var userId = Context.ConnectionId;
            var userHistory = GetUserChatHistory(userId);
            var messages = GetCurrentConversation(userId);
            var tokenCount = GetConversationTotalTokenCount(messages);

            if (string.IsNullOrWhiteSpace(message))
            {
                yield return new StreamedChatMessage { Sender = "System", Message = "Message must be at least 1 character." };
                yield break;
            }

            if (CountTokens(message) > 1000)
            {
                yield return new StreamedChatMessage { Sender = "System", Message = "Message is too long. Please limit your message to 1000 tokens." };
                yield break;
            }

            if (tokenCount.InputTokens > MaxInputTokensPerConversation)
            {
                yield return new StreamedChatMessage { Sender = "System", Message = "This conversation has reached its maximum input token limit. Please start a new conversation." };
                yield break;
            }

            if (tokenCount.OutputTokens > MaxOutputTokensPerConversation)
            {
                yield return new StreamedChatMessage { Sender = "System", Message = "This conversation has reached its maximum output token limit. Please start a new conversation." };
                yield break;
            }

            messages.Add(new ChatMessage(ChatRole.User, message));
            var responseBuilder = new System.Text.StringBuilder();

            await foreach (StreamingChatCompletionUpdate update in _chatClient.CompleteStreamingAsync(messages, new ChatOptions() { MaxOutputTokens = MaxOutputTokensPerConversation }, cancellationToken))
            {
                var text = update.Text ?? "";
                responseBuilder.Append(text);
                int totalOutputTokens = tokenCount.OutputTokens + CountTokens(responseBuilder.ToString());
                if (totalOutputTokens > MaxOutputTokensPerConversation)
                {
                    yield return new StreamedChatMessage { Sender = "System", Message = "This conversation has reached its maximum output token limit. Please start a new conversation.", OutputTokens = totalOutputTokens };
                    yield break;
                }
                yield return new StreamedChatMessage { Sender = "AI", Message = text };
            }

            string response = responseBuilder.ToString();
            messages.Add(new ChatMessage(ChatRole.Assistant, response));

            var totalUsedTokens = GetConversationTotalTokenCount(messages);
            yield return new StreamedChatMessage { Sender = "Counter", InputTokens = totalUsedTokens.InputTokens, OutputTokens = totalUsedTokens.OutputTokens };
        }

        public async Task StartNewChat()
        {
            var userId = Context.ConnectionId;
            var userHistory = GetUserChatHistory(userId);

            if (_env.IsProduction() && !CanStartNewConversation(userHistory.IpAddress))
            {
                await Clients.Caller.SendAsync("ChatReset", "Conversation limit reached for your IP address. Please try again later.");
                return;
            }

            StartNewConversation(userId, userHistory);

            await Clients.Caller.SendAsync("ChatReset");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.ConnectionId;
            _memoryCache.Remove($"chat_{userId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}