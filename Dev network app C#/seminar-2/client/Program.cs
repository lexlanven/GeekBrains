using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Client;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: dotnet run <user-name> <server-ip>");
            return;
        }

        string name = args[0];
        string serverIp = args[1];

        using CancellationTokenSource cts = new();

        while (true)
        {
            Console.WriteLine("Enter message: (type 'Exit' to quit or type 'Send' to send messages): ");
            string input = Console.ReadLine();

            if (input.Equals("Exit", StringComparison.OrdinalIgnoreCase))
            {
                cts.Cancel();
                break;
            }

            if (input.Equals("Send", StringComparison.OrdinalIgnoreCase))
            {
                Task[] tasks = new Task[10];
                for (int i = 0; i < 10; i++)
                {
                    int clientNumber = i + 1;
                    tasks[i] = Task.Run(() => UdpClient(name, serverIp, clientNumber, cts.Token));
                }

                await Task.WhenAll(tasks);
            }
            else
            {
                Console.WriteLine("Invalid input. Type 'Exit' to quit or 'Send' to send messages.");
            }
        }

        Console.WriteLine("Program terminated.");
    }

    static async Task UdpClient(string name, string serverIp, int clientNumber, CancellationToken cancellationToken)
    {
        using UdpClient udpClient = new();
        try
        {
            IPEndPoint serverEndPoint = new(IPAddress.Parse(serverIp), 12345);
            string messageText = "Привет!";

            Message message = new()
            {
                Date = DateTime.Now.ToShortDateString(),
                FromName = name,
                Text = messageText
            };

            string messageJson = message.ToJson();
            byte[] messageBytes = Encoding.ASCII.GetBytes(messageJson);

            await udpClient.SendAsync(messageBytes, messageBytes.Length, serverEndPoint);
            Console.WriteLine($"Client {clientNumber}: Message sent.");

            UdpReceiveResult receiveResult = await udpClient.ReceiveAsync(cancellationToken);
            string receivedData = Encoding.ASCII.GetString(receiveResult.Buffer);
            var receivedMessage = Message.FromJson(receivedData);

            Console.WriteLine($"Client {clientNumber}: Received message from {receivedMessage.FromName} ({receivedMessage.Date}):");
            Console.WriteLine(receivedMessage.Text);

            await Task.Delay(1000, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"Client {clientNumber}: Operation cancelled.");
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Client {clientNumber}: SocketException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client {clientNumber}: Exception: {ex.Message}");
        }
    }
}
