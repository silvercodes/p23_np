
using System.Net;
using System.Net.Sockets;
using System.Text;

const string serverIp = "127.0.0.1";
const int serverPort = 8080;

using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

try
{
    socket.Connect(serverEndPoint);                 // BLOCKING

    while (true)
    {
        Console.Write("> ");
        string? message = Console.ReadLine();

        socket.Send(Encoding.UTF8.GetBytes(message));

        string response = ReadMessage(socket);
        Console.WriteLine($"Response: {response}");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    socket.Shutdown(SocketShutdown.Both);
    socket.Close();
}

string ReadMessage(Socket remoteSocket)
{
    byte[] buffer = new byte[1024];
    int byteCount = 0;
    string message = string.Empty;

    do
    {
        byteCount = remoteSocket.Receive(buffer);       // BLOCKING
        message += Encoding.UTF8.GetString(buffer, 0, byteCount);

    } while (remoteSocket.Available > 0);

    return message;
}

