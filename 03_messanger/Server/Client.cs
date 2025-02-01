using System.Net.Sockets;
using MTP;
using MTypes;

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
                ProtoMessage<AuthRequestPayload> protoMessage = builder.Receive<AuthRequestPayload>();
                Console.WriteLine("ProtoMessage received...");
                //
                //

                AuthRequestPayload? p = protoMessage.GetPayload();

              

                
            }


        }
        catch (Exception)
        {

            throw;
        }
    }
}
