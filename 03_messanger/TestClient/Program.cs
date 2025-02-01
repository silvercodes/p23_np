

using MTP;
using MTypes;
using System.Net.Sockets;

const string host = "127.0.0.1";
const int port = 8080;

try
{
    Console.WriteLine("Press Enter to Connect");
    Console.ReadLine();

    using TcpClient tcpClient = new TcpClient(host, port);
    using NetworkStream netStream = tcpClient.GetStream();

    ProtoMessage<AuthRequestPayload> pm = new ProtoMessage<AuthRequestPayload>()
    {
        Action = "auth"
    };
    pm.SetHeader("key", "iwileouir0293oi");
    pm.SetHeader("ses_id", "3456876");

    pm.SetPayload(new AuthRequestPayload("vasia@mail.com", "123123123"));

    while (true)
    {
        MemoryStream memStream = pm.GetStream();
        Console.WriteLine("Press Enter to Send");
        Console.ReadLine();
        Console.WriteLine(memStream.Length);
        memStream.CopyTo(netStream);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}




