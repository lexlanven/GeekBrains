using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace UdpChatServer
{
    class Program
    {
        private const int ListenPort = 11000;

        static async Task Main(string[] args)
        {
            using var context = new ApplicationDbContext();
            context.Database.Migrate();

            UdpClient listener = new UdpClient(ListenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, ListenPort);

            Console.WriteLine("Waiting for broadcast");

            try
            {
                while (true)
                {
                    byte[] bytes = listener.Receive(ref groupEP);
                    string receivedData = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    Console.WriteLine($"{receivedData} from {groupEP}");

                    if (receivedData.StartsWith("SEND:"))
                    {
                        string messageContent = receivedData.Substring(5);
                        string[] parts = messageContent.Split('|');
                        string username = parts[0];
                        string message = parts[1];

                        var chatMessage = new ChatMessage
                        {
                            Username = username,
                            Message = message,
                            Timestamp = DateTime.UtcNow
                        };

                        await SaveMessageAsync(context, chatMessage);
                        Console.WriteLine("Message saved.");
                    }
                    else if (receivedData.StartsWith("RECEIVE:"))
                    {
                        string username = receivedData.Substring(8);
                        var unreadMessages = await GetUnreadMessagesAsync(context, username);

                        foreach (var msg in unreadMessages)
                        {
                            string response = $"{msg.Timestamp.ToString("HH:mm")} {msg.Username}: {msg.Message}";
                            byte[] sendBytes = Encoding.UTF8.GetBytes(response);
                            listener.Send(sendBytes, sendBytes.Length, groupEP);
                            msg.IsRead = true;
                        }

                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }

        private static async Task SaveMessageAsync(ApplicationDbContext context, ChatMessage chatMessage)
        {
            context.ChatMessages.Add(chatMessage);
            await context.SaveChangesAsync();
        }

        private static async Task<List<ChatMessage>> GetUnreadMessagesAsync(ApplicationDbContext context, string username)
        {
            return await context.ChatMessages
                          .Where(m => m.Username == username && !m.IsRead)
                          .ToListAsync();
        }
    }
}



