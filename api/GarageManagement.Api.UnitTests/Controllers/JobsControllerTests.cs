using GarageManagement.Api.Controllers;
using GarageManagement.Api.Data;
using GarageManagement.Api.DTOs;
using GarageManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarageManagement.Api.UnitTests.Controllers;

[TestFixture]
public class JobsControllerTests
{
    private AppDbContext _db = null!;
    private JobsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _controller = new JobsController(_db);
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose();
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
        _db.Jobs.AddRange(
            new Job { Description = "Older job", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Job { Description = "Newer job", CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) });
        await _db.SaveChangesAsync();

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
        var job = new Job
        {
            Description = "Brake inspection",
            Registration = "AB12 CDE",
            Status = "Unassigned",
            CreatedAt = DateTime.UtcNow
        };
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();

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
    public async Task CreateJob_PersistsJobWithUnassignedStatus()
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
        Assert.That(response.Status, Is.EqualTo("Unassigned"));
        Assert.That(response.AssignedTo, Is.EqualTo("Emma Richardson"));
        Assert.That(_db.Jobs.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task CreateJob_TrimsWhitespaceFromFields()
    {
        var request = new CreateJobRequest
        {
            Description = "  Oil change  ",
            Registration = " AB12 CDE ",
            CustomerName = " Jane Smith "
        };

        var result = await _controller.CreateJob(request);
        var response = (result.Result as CreatedAtActionResult)!.Value as JobResponse;

        Assert.That(response!.Description, Is.EqualTo("Oil change"));
        Assert.That(response.Registration, Is.EqualTo("AB12 CDE"));
        Assert.That(response.CustomerName, Is.EqualTo("Jane Smith"));
    }
}
