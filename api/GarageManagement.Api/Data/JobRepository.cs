using GarageManagement.Api.Models;
using Microsoft.Data.Sqlite;

namespace GarageManagement.Api.Data;

public class JobRepository(IConfiguration configuration) : IJobRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

    public async Task<IReadOnlyList<Job>> GetAllAsync()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT "Id", "Description", "Registration", "Make", "Model", "CustomerName", "AssignedTo", "Status", "CreatedAt"
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
        command.CommandText = """
            SELECT "Id", "Description", "Registration", "Make", "Model", "CustomerName", "AssignedTo", "Status", "CreatedAt"
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
            INSERT INTO "Jobs" ("Description", "Registration", "Make", "Model", "CustomerName", "AssignedTo", "Status", "CreatedAt")
            VALUES ($description, $registration, $make, $model, $customerName, $assignedTo, $status, $createdAt);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$description", job.Description);
        command.Parameters.AddWithValue("$registration", (object?)job.Registration ?? DBNull.Value);
        command.Parameters.AddWithValue("$make", (object?)job.Make ?? DBNull.Value);
        command.Parameters.AddWithValue("$model", (object?)job.Model ?? DBNull.Value);
        command.Parameters.AddWithValue("$customerName", (object?)job.CustomerName ?? DBNull.Value);
        command.Parameters.AddWithValue("$assignedTo", (object?)job.AssignedTo ?? DBNull.Value);
        command.Parameters.AddWithValue("$status", job.Status);
        command.Parameters.AddWithValue("$createdAt", job.CreatedAt.ToString("O"));

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());
        job.Id = id;
        return job;
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    private static Job ReadJob(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Description = reader.GetString(1),
        Registration = reader.IsDBNull(2) ? null : reader.GetString(2),
        Make = reader.IsDBNull(3) ? null : reader.GetString(3),
        Model = reader.IsDBNull(4) ? null : reader.GetString(4),
        CustomerName = reader.IsDBNull(5) ? null : reader.GetString(5),
        AssignedTo = reader.IsDBNull(6) ? null : reader.GetString(6),
        Status = reader.GetString(7),
        CreatedAt = DateTime.Parse(reader.GetString(8), null, System.Globalization.DateTimeStyles.RoundtripKind)
    };
}
