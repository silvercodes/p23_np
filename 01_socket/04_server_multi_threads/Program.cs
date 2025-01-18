
using _04_server_multi_threads;

const string ip = "127.0.0.1";
const int port = 8080;

Server server = new Server(ip, port);
await server.StartAsync();

