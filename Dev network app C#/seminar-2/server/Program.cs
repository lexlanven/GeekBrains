using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Server;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: dotnet run <server-name>");
            return;
        }

        string serverName = args[0];
        using CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;

        var serverTask = Task.Run(() => UdpServer(serverName, token), token);

        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();
        cts.Cancel();

        try
        {
            await serverTask;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Server stopped.");
        }
    }

    static void UdpServer(string serverName, CancellationToken token)
    {
        using UdpClient udpServer = new(12345);

        try
        {
            while (!token.IsCancellationRequested)
            {
                if (udpServer.Available > 0)
                {
                    IPEndPoint clientEndPoint = new(IPAddress.Any, 0);
                    byte[] receiveBytes = udpServer.Receive(ref clientEndPoint);
                    string receivedData = Encoding.ASCII.GetString(receiveBytes);

                    //var receivedMessage = Message.FromJson(receivedData);
                    //Console.WriteLine($"Received message from {receivedMessage.FromName} ({receivedMessage.Date}):");
                    //Console.WriteLine(receivedMessage.Text);

                    string replyMessageText = "Сообщение получено";
                    Message replyMessage = new()
                    {
                        Date = DateTime.Now.ToShortTimeString(),
                        FromName = serverName,
                        Text = replyMessageText
                    };
                    string replyJson = replyMessage.ToJson();
                    byte[] replyBytes = Encoding.ASCII.GetBytes(replyJson);

                    udpServer.Send(replyBytes, replyBytes.Length, clientEndPoint);
                    //Console.WriteLine("Sent response back to client.");
                }
                else
                {
                    Thread.Sleep(100);
                }
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
