using MySql.Data.MySqlClient;

public class Database
{
    private static Database? instance = null;
    private MySqlConnection? conn = null;
    private Database() { }
    ~Database() {}
    public static void Connect()
    {
        if (instance == null)
        {
            instance = new Database();
        }
        instance.conn = new MySqlConnection(Config.DBConnectionString);
        instance.conn.Open();
        Console.WriteLine("Connected to DB\n");
    }
    public static void Disconnect()
    {
        if (instance == null || instance.conn == null)
        {
            return;
        }
        instance.conn.Close();
    }
    public static void Migrate()
    {
        Migrator.Run(Config.DBConnectionString);
    }
    private static void assureInstance()
    {
        if (instance != null)
        {
            return;
        }
        instance = new Database();
    }
    public static string[]? RunCommandGetOne(string query, string[] parameters, object[] values)
    {
        if (instance == null || instance.conn == null)
        {
            return null;
        }

        MySqlCommand command = new MySqlCommand(query, instance.conn);
        for (int i = 0; i < parameters.Length; i++)
        {
            command.Parameters.AddWithValue(parameters[i], values[i]);
        }
        command.Prepare();
        command.ExecuteNonQuery();

        MySqlDataReader reader = command.ExecuteReader();
        reader.Read();

        if (!reader.HasRows) {
            reader.Close();
            return null;
        }

        string[] result = new string[reader.FieldCount];

        for (int i = 0; i < reader.FieldCount; i++)
        {
            result[i] = reader[i].ToString() ?? "";
        }

        reader.Close();

        return result;
    }
    public static List<string[]>? RunCommandGetAll(string query, string[] parameters, object[] values)
    {
        if (instance == null || instance.conn == null)
        {
            return null;
        }

        MySqlCommand command = new MySqlCommand(query, instance.conn);
        for (int i = 0; i < parameters.Length; i++)
        {
            command.Parameters.AddWithValue(parameters[i], values[i]);
        }
        command.Prepare();
        command.ExecuteNonQuery();

        MySqlDataReader reader = command.ExecuteReader();
        
        List<string[]> result = new List<string[]>();
        while(reader.Read()) {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[i] = reader[i].ToString() ?? "";
            }
            result.Add(row);
        }
        reader.Close();
        return result;
    }
    public static void RunCommandWrite(string query, string[] parameters, object[] values)
    {
        if (instance == null || instance.conn == null)
        {
            return;
        }

        MySqlCommand command = new MySqlCommand(query, instance.conn);
        for (int i = 0; i < parameters.Length; i++)
        {
            command.Parameters.AddWithValue(parameters[i], values[i]);
        }
        command.Prepare();
        command.ExecuteNonQuery();
    }
}