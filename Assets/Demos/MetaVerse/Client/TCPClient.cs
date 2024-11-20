using System.Net.Sockets;
using System.Text;

public class TCPClient
{
    private TcpClient _tcpClient;
    public void Connect(string ip, int port)
    {
        _tcpClient = new TcpClient();
        _tcpClient.Connect(ip, port);
        SendData("CONNECT"); // Ex: un message indiquant la connexion.
    }

    private void SendData(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        _tcpClient.GetStream().Write(data, 0, data.Length);
    }
}
