using KendoCRUDService.Filters;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using SharpToken;

namespace KendoCRUDService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LLMController : Controller
    {
        private const string DefaultSystemPrompt = 
            @"You are an AI assistant designed to help users with various tasks and provide clear, accurate, and useful information. 
                Your goal is to understand the user's input, respond politely, and guide them toward the most appropriate and helpful solution. 
                Be concise, professional, and always remain neutral and respectful. 
                Avoid giving opinions or taking sides in discussions, and focus on providing valuable insights. 
                If you do not have enough information to respond accurately, ask the user for clarification.";

        private readonly IChatClient _chatClient;

        public LLMController(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        [HttpPost]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        public async Task<IActionResult> AIChatCompletion([FromBody] IList<ChatMessage> messages)
        {
            var options = new ChatOptions
            {
                MaxOutputTokens = 500,
                Temperature = 0.7f
            };

            ValidateMessagesLength(messages);

            var hasSystemPrompt = messages.Any(x => x.Role == ChatRole.System);

            if (!hasSystemPrompt)
            {
                messages.Prepend(new ChatMessage(ChatRole.System, DefaultSystemPrompt));
            }

            var response = await _chatClient.CompleteAsync(messages, options);

            return Json(response);
        }


        private void ValidateMessagesLength(IList<ChatMessage> messages)
        {
            var tokenizer = GptEncoding.GetEncoding("cl100k_base");

            foreach (var message in messages)
            {
                var tokens = tokenizer.Encode(message.Text);

                if (tokens.Count > 1000) 
                {
                    var truncatedTokens = tokens.Take(1000).ToList();
                    message.Text = tokenizer.Decode(truncatedTokens);
                }
            }
        }
    }
}
