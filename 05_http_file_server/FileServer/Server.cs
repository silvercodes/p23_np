using System.Net;
using System.Text;
using HeyRed.Mime;

namespace FileServer;

internal class Server
{
    private HttpListener listener = null!;
    private string host = "127.0.0.1";
    private int port;
    private string[] indexFiles =
        [
            "index.html",
        ];
    public required string RootDirectory { get; set; }

    public Server(int port = 80)
    {
        this.port = port;

        InitServer();
    }
    private void InitServer()
    {
        listener = new HttpListener();
        listener.Prefixes.Add($@"http://{host}:{port}/");           // <-- LAST SLASH!
    }

    public async Task StartAsync()
    {
        listener.Start();
        Console.WriteLine($"Server started at http://{host}:{port}");

        while (true)
        {
            try
            {
                HttpListenerContext ctx = await listener.GetContextAsync();

                _ = Task.Run(() => HandleRequest(ctx));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
        }
    }

    private async Task HandleRequest(HttpListenerContext ctx)
    {
        string? path = ctx.Request.Url?.AbsolutePath;

        await Console.Out.WriteLineAsync($"Path requsted: {path}");

        path = path.Trim('/');

        if (string.IsNullOrEmpty(path))
        {
            foreach (string indexFile in indexFiles)
            {
                if (File.Exists(Path.Combine(RootDirectory, indexFile)))
                {
                    path = indexFile;
                    break;
                }
            }
        }

        string filePath = Path.Combine(RootDirectory, path);

        if (Path.HasExtension(".html"))
        {
            using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using StreamReader sr = new StreamReader(fs);

            string? content = sr.ReadToEnd();

            string[] dirs = Directory.GetDirectories(Path.GetDirectoryName(filePath));

            StringBuilder sb = new StringBuilder();
            foreach(string dir in dirs)
            {
                string dirname = Path.GetFileName(dir);

                sb.Append($@"<p><a href='{dirname}'>{dirname.ToUpper()}</a></p>");
            }

            content = content.Replace("{{folders}}", sb.ToString());

            byte[] buffer = Encoding.UTF8.GetBytes(content);
            // получаем поток ответа и пишем в него ответ
            ctx.Response.ContentLength64 = buffer.Length;
            using Stream output = ctx.Response.OutputStream;
            // отправляем данные
            await output.WriteAsync(buffer);
            await output.FlushAsync();


        }
        else if (File.Exists(filePath))
        {
            using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            ctx.Response.ContentLength64 = fs.Length;
            ctx.Response.ContentType = MimeTypesMap.GetMimeType(filePath);

            DateTime lm = File.GetLastWriteTime(filePath);
            ctx.Response.AddHeader("Last-Modified", $"{lm.ToShortDateString()} {lm.ToLongTimeString()}");

            ctx.Response.AddHeader("X-Custom", "Hello");

            await fs.CopyToAsync(ctx.Response.OutputStream);

            ctx.Response.StatusCode = (int)HttpStatusCode.OK;

            await fs.FlushAsync();
        }
        else
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        ctx.Response.Close();
    }
}
