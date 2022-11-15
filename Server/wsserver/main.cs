
using WebSocketSharp.Server;

class WsServer {
    // Const messages that server can send ---------
    public const string EventUserListUpdated = "user_list_updated";
    // ---------------------------------------------
    private static WsServer? instance = null;
    private WebSocketServer? wss = null;
    public static void Start() {
        if (instance == null)
        {
            instance = new WsServer();
        }
        instance.wss = new WebSocketServer(Config.ServerWSPort);
        instance.wss.AddWebSocketService<WSConnection>("/test");
        instance.wss.Start();
    }
    public static void Stop() {
        if (instance == null || instance.wss == null) {
            return;
        }
        instance.wss.Stop();
    }
    public static void SendMessageToSession(string id, string message) {
        if (instance == null || instance.wss == null) {
            return;
        }
        instance.wss.WebSocketServices["/test"].Sessions.SendTo(message, id);
    }
    public static void CloseSession(string id) {
        if (instance == null || instance.wss == null) {
            return;
        }
        instance.wss.WebSocketServices["/test"].Sessions.CloseSession(id);
    }
}