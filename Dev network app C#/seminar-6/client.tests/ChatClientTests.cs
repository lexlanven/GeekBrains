using Moq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UdpChatClient.Tests
{
    public class ChatClientTests
    {
        private const int ServerPort = 11000;
        private const string ServerAddress = "localhost";

        [Fact]
        public async Task TestSendMessageAsync()
        {
            var mockUdpClient = new Mock<IUdpClient>();
            string username = "lexlanven";
            string message = "Hello, GeekBrains!";
            string expectedSendString = $"SEND:{username}|{message}";
            byte[] expectedSendBytes = Encoding.UTF8.GetBytes(expectedSendString);

            await SendMessageAsync(mockUdpClient.Object, username, message);

            mockUdpClient.Verify(client => client.SendAsync(expectedSendBytes, expectedSendBytes.Length, ServerAddress, ServerPort), Times.Once);
        }

        [Fact]
        public async Task TestReceiveMessagesAsync_NoUnreadMessages()
        {
            var mockUdpClient = new Mock<IUdpClient>();
            string username = "lexlanven";
            string receiveString = $"RECEIVE:{username}";
            byte[] sendBytes = Encoding.UTF8.GetBytes(receiveString);
            mockUdpClient.Setup(client => client.ReceiveAsync()).ThrowsAsync(new SocketException());

            var output = await ReceiveMessagesAsync(mockUdpClient.Object, username);

            Assert.Equal("No unread messages.", output);
        }

        [Fact]
        public async Task TestReceiveMessagesAsync_WithUnreadMessages()
        {
            var mockUdpClient = new Mock<IUdpClient>();
            string username = "lexlanven";
            string receiveString = $"RECEIVE:{username}";
            byte[] sendBytes = Encoding.UTF8.GetBytes(receiveString);
            string[] messages = { "Message 1", "Message 2" };
            var receiveResults = new Queue<UdpReceiveResult>(messages.Select(msg =>
                new UdpReceiveResult(Encoding.UTF8.GetBytes(msg), new IPEndPoint(IPAddress.Any, 0))));

            mockUdpClient.Setup(client => client.ReceiveAsync()).ReturnsAsync(() => receiveResults.Count > 0 ? receiveResults.Dequeue() : throw new SocketException());

            var output = await ReceiveMessagesAsync(mockUdpClient.Object, username);

            Assert.Equal("Message 1\nMessage 2", output);
        }

        private static async Task SendMessageAsync(IUdpClient client, string username, string message)
        {
            string sendString = $"SEND:{username}|{message}";
            byte[] sendBytes = Encoding.UTF8.GetBytes(sendString);
            await client.SendAsync(sendBytes, sendBytes.Length, ServerAddress, ServerPort);
        }

        private static async Task<string> ReceiveMessagesAsync(IUdpClient client, string username)
        {
            string receiveString = $"RECEIVE:{username}";
            byte[] sendBytes = Encoding.UTF8.GetBytes(receiveString);
            await client.SendAsync(sendBytes, sendBytes.Length, ServerAddress, ServerPort);

            var from = new IPEndPoint(IPAddress.Any, 0);
            bool hasMessages = false;
            client.SetReceiveTimeout(2000);

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                try
                {
                    var receiveResult = await client.ReceiveAsync();
                    string receivedMessage = Encoding.UTF8.GetString(receiveResult.Buffer);
                    if (string.IsNullOrEmpty(receivedMessage)) break;
                    sb.AppendLine(receivedMessage);
                    hasMessages = true;
                }
                catch (SocketException)
                {
                    break;
                }
            }

            return hasMessages ? sb.ToString().Trim() : "No unread messages.";
        }
    }
}
