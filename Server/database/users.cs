public class User
{
    private const string QueryByUsername = "SELECT id, username, password FROM users WHERE username = @username";
    private const string QueryByID = "SELECT id, username, password FROM users WHERE id = @id";
    private const string QueryUpsert = "INSERT INTO users (id, username, password) VALUES (@id, @username, @password) ON DUPLICATE KEY UPDATE username = @username, password = @password";
    public string ID;
    public string Username;
    public string Password;
    public User(string id, string username, string password) {
        this.ID = id;
        this.Username = username;
        this.Password = password;
    }
    public static User FindByUsername(string username) {
        string[] parameters = new string[] {"@username"};
        object[] values = new object[] {username};
        string[]? result = Database.RunCommandGetOne(QueryByUsername, parameters, values);
        if (result == null) {
            return new User("","","");
        }
        return new User(result[0], result[1], result[2]);
    }
    public static User FindByID(string id) {
        string[] parameters = new string[] {"@id"};
        object[] values = new object[] {id};
        string[]? result = Database.RunCommandGetOne(QueryByID, parameters, values);
        if (result == null) {
            return new User("","","");
        }
        return new User(result[0], result[1], result[2]);
    }
    public void Save() {
        string[] parameters = new string[] {"@id","@username","@password"};
        object[] values = new object[] {ID,Username,Password};
        Database.RunCommandWrite(QueryUpsert, parameters, values);
    }
}