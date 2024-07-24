using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Client
{
    public sealed class UdpClientSingleton
    {
        private static readonly Lazy<UdpClientSingleton> lazy = new(() => new UdpClientSingleton());
        public static UdpClientSingleton Instance => lazy.Value;

        private readonly UdpClient udpClient;

        private UdpClientSingleton()
        {
            udpClient = new UdpClient();
        }

        public async Task SendMessageAsync(string name, string serverIp, int clientNumber, CancellationToken cancellationToken)
        {
            try
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), 12345);
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
}
