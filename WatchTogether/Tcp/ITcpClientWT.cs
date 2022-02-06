using System.Net.Sockets;
using System.Threading.Tasks;

namespace WatchTogether.Tcp
{
    internal interface ITcpClientWT
    {
        TcpClient Client { get; }
        bool IsConnected { get; }

        Task ConnectAsync(string hostNameOrIpAddress, int port);
        void Disconnect();
        Task WriteAsync(byte[] data);
        Task WriteAsync(string data);
        Task WriteLineAsync(string data);
    }
}
