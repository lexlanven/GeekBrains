namespace UdpChatServer;

public class ChatMessage
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; } = false;
}