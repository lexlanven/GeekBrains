using System.Net.Sockets;
using System.Threading.Tasks;

namespace UdpChatClient
{
    public class UdpClientWrapper : IUdpClient
    {
        private readonly UdpClient _udpClient;

        public UdpClientWrapper()
        {
            _udpClient = new UdpClient();
        }

        public Task<int> SendAsync(byte[] datagram, int bytes, string hostname, int port)
        {
            return _udpClient.SendAsync(datagram, bytes, hostname, port);
        }

        public Task<UdpReceiveResult> ReceiveAsync()
        {
            return _udpClient.ReceiveAsync();
        }

        public void SetReceiveTimeout(int timeout)
        {
            _udpClient.Client.ReceiveTimeout = timeout;
        }
    }
}
