using System.Net.Sockets;
using System.Threading.Tasks;

namespace UdpChatClient
{
    public interface IUdpClient
    {
        Task<int> SendAsync(byte[] datagram, int bytes, string hostname, int port);
        Task<UdpReceiveResult> ReceiveAsync();
        void SetReceiveTimeout(int timeout);
    }
}
