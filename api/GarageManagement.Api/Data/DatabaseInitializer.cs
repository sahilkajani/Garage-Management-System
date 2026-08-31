using Microsoft.Data.Sqlite;

namespace GarageManagement.Api.Data;

public static class DatabaseInitializer
{
    private const string CreateJobsTableSql = """
        CREATE TABLE IF NOT EXISTS "Jobs" (
            "Id" INTEGER NOT NULL CONSTRAINT "PK_Jobs" PRIMARY KEY AUTOINCREMENT,
            "Description" TEXT NOT NULL,
            "Condition" TEXT NULL,
            "Miles" INTEGER NULL,
            "Critical" TEXT NULL,
            "Registration" TEXT NULL,
            "Make" TEXT NULL,
            "Model" TEXT NULL,
            "CustomerName" TEXT NULL,
            "AssignedTo" TEXT NULL,
            "Status" TEXT NOT NULL,
            "ScheduledDate" TEXT NULL,
            "CompletedDate" TEXT NULL,
            "CreatedAt" TEXT NOT NULL
        );
        """;

    private static readonly (string Name, string Sql)[] ColumnMigrations =
    [
        ("Condition", """ALTER TABLE "Jobs" ADD COLUMN "Condition" TEXT NULL;"""),
        ("Miles", """ALTER TABLE "Jobs" ADD COLUMN "Miles" INTEGER NULL;"""),
        ("Critical", """ALTER TABLE "Jobs" ADD COLUMN "Critical" TEXT NULL;"""),
        ("ScheduledDate", """ALTER TABLE "Jobs" ADD COLUMN "ScheduledDate" TEXT NULL;"""),
        ("CompletedDate", """ALTER TABLE "Jobs" ADD COLUMN "CompletedDate" TEXT NULL;""")
    ];

    public static void Initialize(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        Initialize(connection);
    }

    public static void Initialize(SqliteConnection connection)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = CreateJobsTableSql;
            command.ExecuteNonQuery();
        }

        foreach (var (name, sql) in ColumnMigrations)
        {
            if (ColumnExists(connection, name))
            {
                continue;
            }

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        using (var command = connection.CreateCommand())
        {
            command.CommandText = """
                UPDATE "Jobs"
                SET "Status" = 'Unscheduled'
                WHERE "Status" = 'Unassigned';
                """;
            command.ExecuteNonQuery();
        }
    }

    private static bool ColumnExists(SqliteConnection connection, string columnName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """PRAGMA table_info("Jobs");""";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
