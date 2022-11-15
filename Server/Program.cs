using System.Net;

public class Program
{
    public static bool ShutDownSignal = false;
    public static void Main(string[] args)
    {
        Console.WriteLine("Initializing");

        Config.Load();
        Database.Connect();
        Database.Migrate();

        Thread wsServerThread = new Thread(wsServer);
        Thread httpServerThread = new Thread(httpServer);

        wsServerThread.Start();
        httpServerThread.Start();

        System.Threading.Thread.Sleep(100);
        Console.WriteLine("Press any key to stop servers");

        Console.ReadKey(true);

        Console.WriteLine("Shutting down servers");
        ShutDownSignal = true;
        System.Threading.Thread.Sleep(100);

        Console.WriteLine("Closing database connection");
        Database.Disconnect();
        Console.WriteLine("Database connection closed");

        Console.WriteLine("Press any key to exit program");

        Console.ReadKey(true);
    }

    private static void wsServer()
    {
        WsServer.Start();
        Console.WriteLine("WebSocket server started");

        while (!ShutDownSignal) { }

        WsServer.Stop();
    }

    private static void httpServer()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://127.0.0.1:" + Config.ServerHTTPPort.ToString() + "/");
        listener.Start();

        Console.WriteLine("HTTP server started");

        while (!ShutDownSignal)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string methodAndUrl = String.Format("[{0}][{1}]", request.HttpMethod, request.RawUrl);

            if (request.HttpMethod == "OPTIONS")
            {
                HandlersHTTP.Options(request, response);
                continue;
            }

            switch (methodAndUrl)
            {
                case "[POST][/register]":
                    HandlersHTTP.CreateUser(request, response);
                    break;
                case "[POST][/authorize]":
                    HandlersHTTP.Authorize(request, response);
                    break;
                case "[POST][/logout]":
                    HandlersHTTP.Logout(request, response);
                    break;
                case "[POST][/getInGameUsers]":
                    HandlersHTTP.GetInGameUsers(request, response);
                    break;
                default:
                    HandlersHTTP.NotFound(request, response);
                    break;
            }
        }

        listener.Stop();
    }
}
