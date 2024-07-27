using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpChatClient
{
    class Program
    {
        private const int ServerPort = 11000;
        private const string ServerAddress = "127.0.0.1";
        private static readonly UdpClient Client = new UdpClient();

        static async Task Main()
        {
            Console.WriteLine("Enter your username:");
            string username = Console.ReadLine();

            Console.WriteLine("Enter 'send' to send a message or 'receive' to receive unread messages.");

            while (true)
            {
                string command = Console.ReadLine()?.ToLower();
                if (command == "send")
                {
                    await SendMessageAsync(username);
                }
                else if (command == "receive")
                {
                    await ReceiveMessagesAsync(username);
                }
                else
                {
                    Console.WriteLine("Unknown command. Enter 'send' to send a message or 'receive' to receive unread messages.");
                }
            }
        }

        private static async Task SendMessageAsync(string username)
        {
            Console.WriteLine("Enter your message:");
            string message = Console.ReadLine();
            string sendString = $"SEND:{username}|{message}";
            byte[] sendBytes = Encoding.UTF8.GetBytes(sendString);
            await Client.SendAsync(sendBytes, sendBytes.Length, ServerAddress, ServerPort);
            Console.WriteLine("Message sent.");
            PromptNextAction();
        }

        private static async Task ReceiveMessagesAsync(string username)
        {
            string receiveString = $"RECEIVE:{username}";
            byte[] sendBytes = Encoding.UTF8.GetBytes(receiveString);
            await Client.SendAsync(sendBytes, sendBytes.Length, ServerAddress, ServerPort);

            var from = new IPEndPoint(IPAddress.Any, 0);
            bool hasMessages = false;
            Client.Client.ReceiveTimeout = 2000;

            while (true)
            {
                try
                {
                    byte[] receiveBytes = Client.Receive(ref from);
                    string receivedMessage = Encoding.UTF8.GetString(receiveBytes);
                    if (string.IsNullOrEmpty(receivedMessage)) break;
                    Console.WriteLine(receivedMessage);
                    hasMessages = true;
                }
                catch (SocketException)
                {
                    break;
                }
            }

            if (!hasMessages)
            {
                Console.WriteLine("No unread messages.");
            }

            PromptNextAction();
        }

        private static void PromptNextAction()
        {
            Console.WriteLine("Enter 'send' to send a message or 'receive' to receive unread messages.");
        }
    }
}
