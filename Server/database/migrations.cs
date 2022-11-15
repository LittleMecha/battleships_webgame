using FluentMigrator;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

[Migration(202112250001)]
public class MG1_UsersTable : Migration
{
    public override void Up()
    {
        Create.Table("users")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("username").AsString().Unique()
            .WithColumn("password").AsString();
    }
    public override void Down()
    {
        Delete.Table("users");
    }
}

[Migration(202112260001)]
public class MG2_UsersTableIDChange : Migration
{
    public override void Up()
    {
        Alter.Table("users").AlterColumn("id").AsString();
    }
    public override void Down()
    {
        Alter.Table("users").AlterColumn("id").AsInt64();
    }
}

[Migration(202112260002)]
public class MG3_UserSessions : Migration
{
    public override void Up()
    {
        Create.Table("user_sessions")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("user_id").AsString().Unique()
            .WithColumn("token").AsString().Unique();
    }
    public override void Down()
    {
        Delete.Table("user_sessions");
    }
}

[Migration(202112260003)]
public class MG4_UserSessionsWsSession : Migration
{
    public override void Up()
    {
        Alter.Table("user_sessions").AddColumn("ws_session").AsString();
    }
    public override void Down()
    {
        Delete.Column("ws_session").FromTable("user_sessions");
    }
}

public class Migrator
{
    private static IServiceProvider CreateServices(string connectionString)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddMySql5()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(MG1_UsersTable).Assembly).For.Migrations()
                .ScanIn(typeof(MG2_UsersTableIDChange).Assembly).For.Migrations()
                .ScanIn(typeof(MG3_UserSessions).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }
    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
    public static void Run(string connectionString)
    {
        var serviceProvider = CreateServices(connectionString);

        using (var scope = serviceProvider.CreateScope())
        {
            Migrator.UpdateDatabase(scope.ServiceProvider);
        }
    }
}
