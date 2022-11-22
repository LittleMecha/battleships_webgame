// rebuild methods - they are from usersessions
public class GameSession
{
    private const string QueryFindAll = "SELECT id, P1ID, P2ID, Date FROM GameInfo";
    private const string QueryFindByP1ID = "SELECT id, P1ID, P2ID, Date FROM GameInfo WHERE P1ID = @P1ID";
    private const string QueryFindByP2ID = "SELECT id, P1ID, P2ID, Date FROM GameInfo WHERE P2ID = @P2ID";
    private const string QueryFindByDate = "SELECT id, P1ID, P2ID, Date FROM GameInfo WHERE Date = @Date";
    private const string QueryUpsert = "INSERT INTO GameInfo (id, P1ID, P2ID, Date) VALUES (@id, @P1ID, @P2ID, @Date)";
    public string ID;
    public string P1ID;
    public string P2ID;
    public string Date;

    public GameSession(string id, string P1ID, string P2ID, string date) {
        this.ID = id;
        this.P1ID = P1ID;
        this.P2ID = P2ID;
        this.Date = date;
    }
    public static GameSession[] FindAll() {
        string[] parameters = new string[] {};
        object[] values = new object[] {};
        List<string[]>? result = Database.RunCommandGetAll(QueryFindAll, parameters, values);
        if (result == null) {
            return new GameSession[0];
        }
        GameSession[] games = new GameSession[result.Count];
        int idx = 0;
        foreach (string[] row in result) {
            games[idx] = new GameSession(row[0], row[1], row[2], row[3]);
            idx++;
        }
        return games;
    }
    public static GameSession FindByP1ID(string P1ID) {
        string[] parameters = new string[] {"@P1ID"};
        object[] values = new object[] {P1ID};
        string[]? result = Database.RunCommandGetOne(QueryFindByP1ID, parameters, values);
        if (result == null) {
            return new GameSession("","","","");
        }
        return new GameSession(result[0], result[1], result[2], result[3]);
    }
    public static GameSession FindByP2ID(string P2ID) {
        string[] parameters = new string[] {"@P2ID"};
        object[] values = new object[] {P2ID};
        string[]? result = Database.RunCommandGetOne(QueryFindByP2ID, parameters, values);
        if (result == null) {
            return new GameSession("","","","");
        }
        return new GameSession(result[0], result[1], result[2], result[3]);
    }
    public static GameSession FindByDate(string wsSession) {
        string[] parameters = new string[] {"@Date"};
        object[] values = new object[] {wsSession};
        string[]? result = Database.RunCommandGetOne(QueryFindByDate, parameters, values);
        if (result == null) {
            return new GameSession("","","","");
        }
        return new GameSession(result[0], result[1], result[2], result[3]);
    }
    public void Save() {
        string[] parameters = new string[] {"@id","@P1ID", "@P2ID", "@Date"};
        object[] values = new object[] {ID,P1ID,P2ID,Date};
        Database.RunCommandWrite(QueryUpsert, parameters, values);
    }
}