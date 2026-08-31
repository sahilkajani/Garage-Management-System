using GarageManagement.Api.Controllers;
using GarageManagement.Api.Data;
using GarageManagement.Api.DTOs;
using GarageManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace GarageManagement.Api.UnitTests.Controllers;

[TestFixture]
public class JobsControllerTests
{
    private SqliteConnection _connection = null!;
    private IJobRepository _repository = null!;
    private JobsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        const string connectionString = "Data Source=JobsControllerTests;Mode=Memory;Cache=Shared";
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        DatabaseInitializer.Initialize(_connection);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            })
            .Build();

        _repository = new JobRepository(configuration);
        _controller = new JobsController(_repository);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Dispose();
    }

    [Test]
    public async Task GetJobs_WhenNoJobs_ReturnsEmptyList()
    {
        var result = await _controller.GetJobs();

        var ok = result.Result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.InstanceOf<IEnumerable<JobResponse>>());
        Assert.That(((IEnumerable<JobResponse>)ok.Value!).Count(), Is.Zero);
    }

    [Test]
    public async Task GetJobs_ReturnsJobsOrderedByCreatedAtDescending()
    {
        await _repository.CreateAsync(new Job
        {
            Description = "Older job",
            Status = "Unscheduled",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
        await _repository.CreateAsync(new Job
        {
            Description = "Newer job",
            Status = "Unscheduled",
            CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        var result = await _controller.GetJobs();

        var ok = result.Result as OkObjectResult;
        var jobs = ((IEnumerable<JobResponse>)ok!.Value!).ToList();

        Assert.That(jobs, Has.Count.EqualTo(2));
        Assert.That(jobs[0].Description, Is.EqualTo("Newer job"));
        Assert.That(jobs[1].Description, Is.EqualTo("Older job"));
    }

    [Test]
    public async Task GetJob_WhenJobExists_ReturnsJob()
    {
        var job = await _repository.CreateAsync(new Job
        {
            Description = "Brake inspection",
            Registration = "AB12 CDE",
            Status = "Unscheduled",
            CreatedAt = DateTime.UtcNow
        });

        var result = await _controller.GetJob(job.Id);

        var ok = result.Result as OkObjectResult;
        var response = ok!.Value as JobResponse;

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Id, Is.EqualTo(job.Id));
        Assert.That(response.Description, Is.EqualTo("Brake inspection"));
        Assert.That(response.Registration, Is.EqualTo("AB12 CDE"));
    }

    [Test]
    public async Task GetJob_WhenJobDoesNotExist_ReturnsNotFound()
    {
        var result = await _controller.GetJob(999);

        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task CreateJob_PersistsJobWithUnscheduledStatus()
    {
        var request = new CreateJobRequest
        {
            Description = "Annual service",
            Registration = "XY99 ZZZ",
            Make = "Ford",
            Model = "Focus",
            CustomerName = "Jane Smith",
            AssignedTo = "Emma Richardson"
        };

        var result = await _controller.CreateJob(request);

        var created = result.Result as CreatedAtActionResult;
        var response = created!.Value as JobResponse;

        Assert.That(created.ActionName, Is.EqualTo(nameof(JobsController.GetJob)));
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Description, Is.EqualTo("Annual service"));
        Assert.That(response.Status, Is.EqualTo("Unscheduled"));
        Assert.That(response.AssignedTo, Is.EqualTo("Emma Richardson"));

        var jobs = await _repository.GetAllAsync();
        Assert.That(jobs, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task CreateJob_TrimsWhitespaceFromFields()
    {
        var request = new CreateJobRequest
        {
            Description = "  Oil change  ",
            Condition = "  On cold start  ",
            Registration = " AB12 CDE ",
            CustomerName = " Jane Smith "
        };

        var result = await _controller.CreateJob(request);
        var response = (result.Result as CreatedAtActionResult)!.Value as JobResponse;

        Assert.That(response!.Description, Is.EqualTo("Oil change"));
        Assert.That(response.Condition, Is.EqualTo("On cold start"));
        Assert.That(response.Registration, Is.EqualTo("AB12 CDE"));
        Assert.That(response.CustomerName, Is.EqualTo("Jane Smith"));
    }

    [Test]
    public async Task CreateJob_PersistsExtendedJobDetails()
    {
        var request = new CreateJobRequest
        {
            Description = "Grinding noise when braking",
            Condition = "On cold start and when braking at low speed",
            Miles = 45210,
            Critical = "high"
        };

        var result = await _controller.CreateJob(request);
        var response = (result.Result as CreatedAtActionResult)!.Value as JobResponse;

        Assert.That(response!.Condition, Is.EqualTo("On cold start and when braking at low speed"));
        Assert.That(response.Miles, Is.EqualTo(45210));
        Assert.That(response.Critical, Is.EqualTo("High"));
    }

    [Test]
    public async Task CreateJob_WithInvalidCritical_ReturnsBadRequest()
    {
        var request = new CreateJobRequest
        {
            Description = "Test job",
            Critical = "Urgent"
        };

        var result = await _controller.CreateJob(request);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateJob_WhenJobExists_UpdatesAndReturnsJob()
    {
        var created = await _repository.CreateAsync(new Job
        {
            Description = "Original description",
            Status = "Unscheduled",
            CreatedAt = DateTime.UtcNow
        });

        var request = new UpdateJobRequest
        {
            Description = "Updated description",
            Condition = "On cold start",
            Miles = 12000,
            Critical = "Medium",
            Status = "Scheduled",
            ScheduledDate = new DateTime(2026, 9, 15, 10, 0, 0, DateTimeKind.Utc),
            CustomerName = "Jane Smith"
        };

        var result = await _controller.UpdateJob(created.Id, request);
        var ok = result.Result as OkObjectResult;
        var response = ok!.Value as JobResponse;

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Description, Is.EqualTo("Updated description"));
        Assert.That(response.Condition, Is.EqualTo("On cold start"));
        Assert.That(response.Miles, Is.EqualTo(12000));
        Assert.That(response.Critical, Is.EqualTo("Medium"));
        Assert.That(response.Status, Is.EqualTo("Scheduled"));
        Assert.That(response.ScheduledDate, Is.EqualTo(new DateTime(2026, 9, 15, 10, 0, 0, DateTimeKind.Utc)));
    }

    [Test]
    public async Task UpdateJob_WithInvalidStatus_ReturnsBadRequest()
    {
        var created = await _repository.CreateAsync(new Job
        {
            Description = "Test job",
            Status = "Unscheduled",
            CreatedAt = DateTime.UtcNow
        });

        var request = new UpdateJobRequest
        {
            Description = "Test job",
            Status = "In Progress"
        };

        var result = await _controller.UpdateJob(created.Id, request);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateJob_WhenJobDoesNotExist_ReturnsNotFound()
    {
        var request = new UpdateJobRequest { Description = "Missing job", Status = "Unscheduled" };

        var result = await _controller.UpdateJob(999, request);

        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }
}
