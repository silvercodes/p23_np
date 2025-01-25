

using MTP;
using System.Net.Sockets;

const string host = "127.0.0.1";
const int port = 8080;


using TcpClient tcpClient = new TcpClient(host, port);
using NetworkStream netStream = tcpClient.GetStream();

ProtoMessage pm = new ProtoMessage()
{
    Action = "auth"
};

try
{
    while(true)
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




