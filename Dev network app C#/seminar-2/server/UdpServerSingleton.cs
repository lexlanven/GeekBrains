using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Server
{
    public sealed class UdpServerSingleton
    {
        private static readonly Lazy<UdpServerSingleton> lazy = new(() => new UdpServerSingleton());
        public static UdpServerSingleton Instance => lazy.Value;

        private UdpClient udpServer;
        private CancellationTokenSource cts;
        private string serverName;

        private UdpServerSingleton() { }

        public void Start(string serverName)
        {
            if (udpServer != null)
            {
                Console.WriteLine("Server is already running.");
                return;
            }

            this.serverName = serverName;
            udpServer = new UdpClient(12345);
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task.Run(() => RunServer(token));
        }

        public void Stop()
        {
            if (udpServer == null)
            {
                Console.WriteLine("Server is not running.");
                return;
            }

            cts.Cancel();
            udpServer.Close();
            udpServer = null;
            Console.WriteLine("Server stopped.");
        }

        private void RunServer(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (udpServer.Available > 0)
                    {
                        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] receiveBytes = udpServer.Receive(ref clientEndPoint);
                        string receivedData = Encoding.ASCII.GetString(receiveBytes);

                        // var receivedMessage = Message.FromJson(receivedData);
                        // Console.WriteLine($"Received message from {receivedMessage.FromName} ({receivedMessage.Date}):");
                        // Console.WriteLine(receivedMessage.Text);
                        string replyMessageText = "Сообщение получено";
                        Message replyMessage = new Message
                        {
                            Date = DateTime.Now.ToShortTimeString(),
                            FromName = serverName,
                            Text = replyMessageText
                        };
                        string replyJson = replyMessage.ToJson();
                        byte[] replyBytes = Encoding.ASCII.GetBytes(replyJson);

                        udpServer.Send(replyBytes, replyBytes.Length, clientEndPoint);
                        // Console.WriteLine("Sent response back to client.");
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
        }
    }
}
