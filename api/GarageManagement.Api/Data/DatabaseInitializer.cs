using Microsoft.Data.Sqlite;

namespace GarageManagement.Api.Data;

public static class DatabaseInitializer
{
    private const string CreateJobsTableSql = """
        CREATE TABLE IF NOT EXISTS "Jobs" (
            "Id" INTEGER NOT NULL CONSTRAINT "PK_Jobs" PRIMARY KEY AUTOINCREMENT,
            "Description" TEXT NOT NULL,
            "Registration" TEXT NULL,
            "Make" TEXT NULL,
            "Model" TEXT NULL,
            "CustomerName" TEXT NULL,
            "AssignedTo" TEXT NULL,
            "Status" TEXT NOT NULL,
            "CreatedAt" TEXT NOT NULL
        );
        """;

    public static void Initialize(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        Initialize(connection);
    }

    public static void Initialize(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = CreateJobsTableSql;
        command.ExecuteNonQuery();
    }
}
