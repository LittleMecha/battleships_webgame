public class UserSession
{
    private const string QueryFindAll = "SELECT id, user_id, token, ws_session FROM user_sessions";
    private const string QueryFindByToken = "SELECT id, user_id, token, ws_session FROM user_sessions WHERE token = @token";
    private const string QueryFindByWsSession = "SELECT id, user_id, token, ws_session FROM user_sessions WHERE ws_session = @ws_session";
    private const string QueryUpsert = "INSERT INTO user_sessions (id, user_id, token, ws_session) VALUES (@id, @user_id, @token, @ws_session) ON DUPLICATE KEY UPDATE ws_session = @ws_session";
    private const string QueryRemove = "DELETE FROM user_sessions WHERE user_id = @user_id";
    public string ID;
    public string UserID;
    public string Token;
    public string WsSession;
    public UserSession(string id, string userID, string token, string wsSession) {
        this.ID = id;
        this.UserID = userID;
        this.Token = token;
        this.WsSession = wsSession;
    }
    public static UserSession[] FindAll() {
        string[] parameters = new string[] {};
        object[] values = new object[] {};
        List<string[]>? result = Database.RunCommandGetAll(QueryFindAll, parameters, values);
        if (result == null) {
            return new UserSession[0];
        }
        UserSession[] sessions = new UserSession[result.Count];
        int idx = 0;
        foreach (string[] row in result) {
            sessions[idx] = new UserSession(row[0], row[1], row[2], row[3]);
            idx++;
        }
        return sessions;
    }
    public static UserSession FindByToken(string token) {
        string[] parameters = new string[] {"@token"};
        object[] values = new object[] {token};
        string[]? result = Database.RunCommandGetOne(QueryFindByToken, parameters, values);
        if (result == null) {
            return new UserSession("","","","");
        }
        return new UserSession(result[0], result[1], result[2], result[3]);
    }
    public static UserSession FindByWsSession(string wsSession) {
        string[] parameters = new string[] {"@ws_session"};
        object[] values = new object[] {wsSession};
        string[]? result = Database.RunCommandGetOne(QueryFindByWsSession, parameters, values);
        if (result == null) {
            return new UserSession("","","","");
        }
        return new UserSession(result[0], result[1], result[2], result[3]);
    }
    public void Save() {
        string[] parameters = new string[] {"@id","@user_id", "@token", "@ws_session"};
        object[] values = new object[] {ID,UserID,Token,WsSession};
        Database.RunCommandWrite(QueryUpsert, parameters, values);
    }
    public void Delete() {
        string[] parameters = new string[] {"@user_id"};
        object[] values = new object[] {UserID};
        Database.RunCommandWrite(QueryRemove, parameters, values);
    }
}