using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.AI;
namespace signalr_for_aspnet_core.Models;

public class UserChatHistory
{
    public List<List<ChatMessage>> Conversations { get; set; } = new();
    public int CurrentConversationIndex { get; set; } = 0;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public string IpAddress { get; set; }
}