using System.Text.Json;

using WebSocketSharp;
using WebSocketSharp.Server;

public class WSConnection : WebSocketBehavior
{
    private void logOk(string id, string command, string message)
    {
        Console.WriteLine("WS [{0}] SUCCESS - {1} - {2}", id, command, message);
    }
    private void logError(string id, string command, string message)
    {
        Console.WriteLine("WS [{0}] ERROR - {1} - {2}", id, command, message);
    }
    private struct WSAPIRequest
    {
        public string command { get; set; }
        public WSAttachRequestData attach { get; set; }
    }
    private struct WSAttachRequestData
    {
        public string token { get; set; }
    }
    protected override void OnMessage(MessageEventArgs msg)
    {
        WSAPIRequest cmd;
        try
        {
            cmd = JsonSerializer.Deserialize<WSAPIRequest>(msg.Data);
        }
        catch (Exception err)
        {
            logError(ID, "", "Unexpected message");
            Console.WriteLine(err);
            Sessions.CloseSession(ID);
            return;
        }
        switch (cmd.command)
        {
            case "attach":
                if (cmd.attach.token.Length == 0)
                {
                    logError(ID, cmd.command, "Missing token, closing session");
                    Sessions.CloseSession(ID);
                    return;
                }
                UserSession userSess = UserSession.FindByToken(cmd.attach.token);
                if (userSess.ID.Length == 0)
                {
                    logError(ID, cmd.command, "User session not found, closing session");
                    Sessions.CloseSession(ID);
                    return;
                }
                if (userSess.WsSession.Length > 0 && userSess.WsSession != ID)
                {
                    logError(ID, cmd.command, "User session already attached, closing session");
                    Sessions.CloseSession(ID);
                    return;
                }
                userSess.WsSession = ID;
                userSess.Save();
                Sessions.Broadcast("{\"event\": \"" + WsServer.EventUserListUpdated + "\"}");
                logOk(ID, cmd.command, "Ws session attached to user session");
                return;
            default:
                logError(ID, cmd.command, "Unknown command, closing session");
                Sessions.CloseSession(ID);
                return;
        }
    }
    protected override void OnClose(CloseEventArgs e)
    {
        UserSession userSess = UserSession.FindByWsSession(ID);
        if (userSess.ID.Length > 0)
        {
            userSess.Delete();
            logOk(ID, "System.RemoveSession", "Session found and removed");
            Sessions.Broadcast("{\"event\": \"" + WsServer.EventUserListUpdated + "\"}");
            return;
        }
        Sessions.Broadcast("{\"event\": \"" + WsServer.EventUserListUpdated + "\"}");
        logOk(ID, "System.RemoveSession", "Session not found, nothing to do");
    }
}