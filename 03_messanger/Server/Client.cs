using MTP;
using System.Net.Sockets;

namespace Server;

internal class Client
{
    private TcpClient tcpClient;
    private NetworkStream netStream = null!;

    public Client(TcpClient tcpClient)
    {
        this.tcpClient = tcpClient;
    }

    public void Processing()
    {
        try
        {
            netStream = tcpClient.GetStream();

            ProtoMessageBuilder builder = new ProtoMessageBuilder(netStream);

            while (true)
            {
                ProtoMessage protoMessage = builder.Receive();


            }


        }
        catch (Exception)
        {

            throw;
        }
    }
}
