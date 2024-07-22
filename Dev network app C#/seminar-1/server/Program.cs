using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Server;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: dotnet run <server-name>");
            return;
        }

        string serverName = args[0];

        UdpServer(serverName);
    }

    static void UdpServer(string serverName)
    {
        UdpClient udpServer = new(12345);

        try
        {
            while (true)
            {
                Console.WriteLine("UDP Server awaiting messages.");
                IPEndPoint clientEndPoint = new(IPAddress.Any, 0);
                byte[] receiveBytes = udpServer.Receive(ref clientEndPoint);
                string receivedData = Encoding.UTF8.GetString(receiveBytes);

                var receivedMessage = Message.FromJson(receivedData);
                Console.WriteLine($"Received message from {receivedMessage.FromName} ({receivedMessage.Date}):");
                Console.WriteLine(receivedMessage.Text);

                string replyMessageText = $"Server received: {receivedMessage.Text}";
                Message replyMessage = new()
                {
                    Date = DateTime.Now.ToShortTimeString(),
                    FromName = serverName,
                    Text = replyMessageText
                };
                string replyJson = replyMessage.ToJson();
                byte[] replyBytes = Encoding.UTF8.GetBytes(replyJson);

                udpServer.Send(replyBytes, replyBytes.Length, clientEndPoint);
                Console.WriteLine("Sent response back to client.");
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
        finally
        {
            udpServer.Close();
        }
    }
}
