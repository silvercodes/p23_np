using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _04_server_multi_threads;

internal class Server: IAsyncDisposable
{
    private int backlog;
    public string Ip { get; }
    public int Port { get; }
    public Socket ServerSocket { get; private set; }
    public Server(string ip, int port, int backlog = 10)
    {
        Ip = ip;
        Port = port;

        ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Ip), Port);
        ServerSocket.Bind(ep);
    }
    
    public async Task StartAsync()
    {
        try
        {
            ServerSocket.Listen(backlog);
            await Console.Out.WriteLineAsync($"Server started at {Ip}:{Port}");

            await HandleAsync();
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"ERROR: {ex.Message}");
        }
    }
    public async Task HandleAsync()
    {
        while(true)
        {
            Socket remoteSocket = await ServerSocket.AcceptAsync();

            if (remoteSocket.RemoteEndPoint is IPEndPoint remoteEP)
                await Console.Out.WriteLineAsync($"Connection opened for: {remoteEP.Address}:{remoteEP.Port}");

            _ = Task.Run(() => HandleConnection(remoteSocket));
        }
    }


    private void HandleConnection(Socket remoteSocket)
    {
        try
        {
            while (true)
            {
                string message = ReadMessage(remoteSocket);

                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} -> {message}");

                Thread.Sleep(1000);

                string response = $"OK ({message})";
                remoteSocket.Send(Encoding.UTF8.GetBytes(response));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in ConnectionHandler: {ex.Message}");
        }
        finally
        {
            remoteSocket.Shutdown(SocketShutdown.Both);
            remoteSocket.Close();

            Console.WriteLine("Connection closed...");
        }
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

    public async ValueTask DisposeAsync()
    {
        await Task.Run(() =>
        {
            if (ServerSocket != null)
                ServerSocket.Dispose();
        });
    }
}
