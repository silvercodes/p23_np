using System.Net;
using System.Net.Sockets;
using System.Text;

const string localhost = "172.20.10.13";
const string remotehost = "172.20.10.13";

Console.Write("Enter a local PORT: ");
int localPort = Int32.Parse(Console.ReadLine());
Console.Write("Enter a remote PORT: ");
int remotePort = Int32.Parse(Console.ReadLine());

Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

_ = Task.Run(() => 
{
    try
    {
        socket.Bind(new IPEndPoint(IPAddress.Parse(localhost), localPort));

        while(true)
        {
            byte[] buffer = new byte[65507];            // Размер буфера МАКСИМАЛЬНЫЙ!!!
            int byteCount = 0;

            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            byteCount = socket.ReceiveFrom(buffer, ref remoteEP);       // BLOCKING
            string message = Encoding.UTF8.GetString(buffer, 0, byteCount);

            if (remoteEP is IPEndPoint remoteEPWithInfo)
                Console.Write($"FROM: {remoteEPWithInfo.Address}:{remoteEPWithInfo.Port} -> ");
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: {message}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Worker: {ex.Message}");
    }
    finally
    {
        socket.Close();
    }
});

try
{
    while(true)
    {
        string? message = Console.ReadLine();

        if (message is null)
            continue;

        byte[] data = Encoding.UTF8.GetBytes(message);
        socket.SendTo(data, new IPEndPoint(IPAddress.Parse(remotehost), remotePort));
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Main: {ex.Message}");
}
finally
{
    socket.Close();
}