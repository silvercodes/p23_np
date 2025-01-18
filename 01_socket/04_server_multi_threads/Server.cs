namespace _04_server_multi_threads;

internal class Server
{
    public string Ip { get; }
    public int Port { get; }
    public Server(string ip, int port)
    {
        Ip = ip;
        Port = port;
    }
    public async Task StartAsync()
    {

    }
}
