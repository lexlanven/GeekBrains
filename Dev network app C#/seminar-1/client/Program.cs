using System.Net;
using System.Net.Sockets;
using System.Text;
using Client;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: dotnet run <user-name> <server-ip>");
            return;
        }

        string name = args[0];
        string serverIp = args[1];

        UdpClient(name, serverIp);
    }

    static void UdpClient(string name, string serverIp)
    {
        using UdpClient udpClient = new();
        try
        {
            IPEndPoint serverEndPoint = new(IPAddress.Parse(serverIp), 12345);

            while (true)
            {
                Console.WriteLine("UDP Client awaiting input.");
                Console.Write("Enter message and press Enter: ");
                string messageText = Console.ReadLine();

                Message message = new()
                {
                    Date = DateTime.Now.ToShortDateString(),
                    FromName = name,
                    Text = messageText
                };
                
                string messageJson = message.ToJson();
                byte[] messageBytes = Encoding.ASCII.GetBytes(messageJson);

                udpClient.Send(messageBytes, messageBytes.Length, serverEndPoint);
                Console.WriteLine("Message sent.");

                byte[] receiveBytes = udpClient.Receive(ref serverEndPoint);
                string receivedData = Encoding.ASCII.GetString(receiveBytes);
                var receivedMessage = Message.FromJson(receivedData);

                Console.WriteLine($"Received message from {receivedMessage.FromName} ({receivedMessage.Date}):");
                Console.WriteLine(receivedMessage.Text);
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}
