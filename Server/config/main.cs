public class Config
{
    // Defaults -----------------------------
    private const string DefaultDbConnectionString = "server=127.0.0.1;user=root;database=testdb;password=secret";
    private const int DefaultHttpServerPort = 8585;
    private const int DefaultWsServerPort = 8686;
    // --------------------------------------
    // Variables ----------------------------
    private string dbConnectionString;
    private int httpServerPort;
    private int wsServerPort;
    // --------------------------------------
    private static Config? instance = null;
    private Config()
    {
        dbConnectionString = "";
    }
    public static void Load()
    {
        if (instance == null)
        {
            instance = new Config();
        }
        instance.dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? DefaultDbConnectionString;

        string? httpPort = Environment.GetEnvironmentVariable("SERVER_HTTP_PORT");
        instance.httpServerPort = httpPort == null ? DefaultHttpServerPort : Int32.Parse(httpPort);

        string? wsPort = Environment.GetEnvironmentVariable("SERVER_WS_PORT");
        instance.wsServerPort = wsPort == null ? DefaultWsServerPort : Int32.Parse(wsPort);
    }
    public static string DBConnectionString
    {
        get
        {
            if (instance == null)
            {
                return "";
            }
            return instance.dbConnectionString;
        }
    }
    public static int ServerHTTPPort
    {
        get
        {
            if (instance == null)
            {
                return 0;
            }
            return instance.httpServerPort;
        }
    }
    public static int ServerWSPort
    {
        get
        {
            if (instance == null)
            {
                return 0;
            }
            return instance.wsServerPort;
        }
    }
}