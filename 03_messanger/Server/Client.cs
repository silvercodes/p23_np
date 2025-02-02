using System.Net.Sockets;
using MTP;
using MTypes;
using Server.Controllers;
using Server.Routing;

namespace Server;

internal class Client
{
    private TcpClient tcpClient;
    private NetworkStream netStream = null!;
    private Router router;

    public Client(TcpClient tcpClient)
    {
        this.tcpClient = tcpClient;
        router = new Router(new List<Route>()
        {
            new Route("auth", typeof(AuthController), "Login"),
        });
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
                //Console.WriteLine("ProtoMessage received...");
                ////
                ////

                //AuthRequestPayload? p = protoMessage.GetPayload();


                router.Handle<AuthRequestPayload>(protoMessage);

              

                
            }


        }
        catch (Exception)
        {

            throw;
        }
    }
}
