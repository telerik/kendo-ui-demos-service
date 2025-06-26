namespace signalr_for_aspnet_core.Models;

public class StreamedChatMessage
{
    public string Sender { get; set; }
    public string Message { get; set; }

    public int InputTokens { get; set; }

    public int OutputTokens { get; set; }
}