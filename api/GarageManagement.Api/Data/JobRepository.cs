using GarageManagement.Api.Models;
using Microsoft.Data.Sqlite;

namespace GarageManagement.Api.Data;

public class JobRepository(IConfiguration configuration) : IJobRepository
{
    private const string JobColumns = """
        "Id", "Description", "Condition", "Miles", "Critical", "Registration", "Make", "Model",
        "CustomerName", "AssignedTo", "Status", "ScheduledDate", "CompletedDate", "CreatedAt"
        """;

    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

    public async Task<IReadOnlyList<Job>> GetAllAsync()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            SELECT {JobColumns}
            FROM "Jobs"
            ORDER BY "CreatedAt" DESC;
            """;

        var jobs = new List<Job>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            jobs.Add(ReadJob(reader));
        }

        return jobs;
    }

    public async Task<Job?> GetByIdAsync(int id)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            SELECT {JobColumns}
            FROM "Jobs"
            WHERE "Id" = $id;
            """;
        command.Parameters.AddWithValue("$id", id);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadJob(reader) : null;
    }

    public async Task<Job> CreateAsync(Job job)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO "Jobs" (
                "Description", "Condition", "Miles", "Critical", "Registration", "Make", "Model",
                "CustomerName", "AssignedTo", "Status", "ScheduledDate", "CompletedDate", "CreatedAt"
            )
            VALUES (
                $description, $condition, $miles, $critical, $registration, $make, $model,
                $customerName, $assignedTo, $status, $scheduledDate, $completedDate, $createdAt
            );
            SELECT last_insert_rowid();
            """;
        AddJobParameters(command, job);
        command.Parameters.AddWithValue("$createdAt", job.CreatedAt.ToString("O"));

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());
        job.Id = id;
        return job;
    }

    public async Task<Job?> UpdateAsync(Job job)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE "Jobs"
            SET
                "Description" = $description,
                "Condition" = $condition,
                "Miles" = $miles,
                "Critical" = $critical,
                "Registration" = $registration,
                "Make" = $make,
                "Model" = $model,
                "CustomerName" = $customerName,
                "AssignedTo" = $assignedTo,
                "Status" = $status,
                "ScheduledDate" = $scheduledDate,
                "CompletedDate" = $completedDate
            WHERE "Id" = $id;
            """;
        command.Parameters.AddWithValue("$id", job.Id);
        AddJobParameters(command, job);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected == 0 ? null : job;
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    private static void AddJobParameters(SqliteCommand command, Job job)
    {
        command.Parameters.AddWithValue("$description", job.Description);
        command.Parameters.AddWithValue("$condition", (object?)job.Condition ?? DBNull.Value);
        command.Parameters.AddWithValue("$miles", (object?)job.Miles ?? DBNull.Value);
        command.Parameters.AddWithValue("$critical", (object?)job.Critical ?? DBNull.Value);
        command.Parameters.AddWithValue("$registration", (object?)job.Registration ?? DBNull.Value);
        command.Parameters.AddWithValue("$make", (object?)job.Make ?? DBNull.Value);
        command.Parameters.AddWithValue("$model", (object?)job.Model ?? DBNull.Value);
        command.Parameters.AddWithValue("$customerName", (object?)job.CustomerName ?? DBNull.Value);
        command.Parameters.AddWithValue("$assignedTo", (object?)job.AssignedTo ?? DBNull.Value);
        command.Parameters.AddWithValue("$status", job.Status);
        command.Parameters.AddWithValue("$scheduledDate", job.ScheduledDate?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$completedDate", job.CompletedDate?.ToString("O") ?? (object)DBNull.Value);
    }

    private static Job ReadJob(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Description = reader.GetString(1),
        Condition = reader.IsDBNull(2) ? null : reader.GetString(2),
        Miles = reader.IsDBNull(3) ? null : reader.GetInt32(3),
        Critical = reader.IsDBNull(4) ? null : reader.GetString(4),
        Registration = reader.IsDBNull(5) ? null : reader.GetString(5),
        Make = reader.IsDBNull(6) ? null : reader.GetString(6),
        Model = reader.IsDBNull(7) ? null : reader.GetString(7),
        CustomerName = reader.IsDBNull(8) ? null : reader.GetString(8),
        AssignedTo = reader.IsDBNull(9) ? null : reader.GetString(9),
        Status = reader.GetString(10),
        ScheduledDate = ReadNullableDateTime(reader, 11),
        CompletedDate = ReadNullableDateTime(reader, 12),
        CreatedAt = DateTime.Parse(reader.GetString(13), null, System.Globalization.DateTimeStyles.RoundtripKind)
    };

    private static DateTime? ReadNullableDateTime(SqliteDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal)
            ? null
            : DateTime.Parse(reader.GetString(ordinal), null, System.Globalization.DateTimeStyles.RoundtripKind);
}
