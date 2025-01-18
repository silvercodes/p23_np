using System.Net;
using System.Net.Sockets;
using System.Text;


const string serverIp = "127.0.0.1";
const int serverPort = 8080;

using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

try
{
    socket.Bind(endPoint);
    socket.Listen();

    Console.WriteLine($"Server started at {serverIp}:{serverPort}");

    while(true)
    {
        Socket remoteSocket = socket.Accept();            // BLOCKING
        Console.WriteLine("Connection opened...");

        string message = ReadMessage(remoteSocket);

        Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: {message}");

        Thread.Sleep(2000);

        string response = "Hello from server! All OK!";
        remoteSocket.Send(Encoding.UTF8.GetBytes(response));

        remoteSocket.Shutdown(SocketShutdown.Both);
        remoteSocket.Close();

        Console.WriteLine("Connection closed...");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
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



