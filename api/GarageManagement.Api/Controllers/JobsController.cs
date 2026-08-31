using GarageManagement.Api.Data;
using GarageManagement.Api.DTOs;
using GarageManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GarageManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController(IJobRepository jobs) : ControllerBase
{
    private static readonly HashSet<string> AllowedCriticalLevels = new(StringComparer.OrdinalIgnoreCase)
    {
        "High", "Medium", "Low"
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobResponse>>> GetJobs()
    {
        var jobList = await jobs.GetAllAsync();
        return Ok(jobList.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobResponse>> GetJob(int id)
    {
        var job = await jobs.GetByIdAsync(id);
        if (job is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(job));
    }

    [HttpPost]
    public async Task<ActionResult<JobResponse>> CreateJob([FromBody] CreateJobRequest request)
    {
        if (!TryNormalizeCritical(request.Critical, out var critical, out var criticalError))
        {
            return criticalError!;
        }

        var job = MapRequestToJob(request, critical);
        job.Status = "Unassigned";
        job.CreatedAt = DateTime.UtcNow;

        var createdJob = await jobs.CreateAsync(job);

        return CreatedAtAction(nameof(GetJob), new { id = createdJob.Id }, ToResponse(createdJob));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<JobResponse>> UpdateJob(int id, [FromBody] UpdateJobRequest request)
    {
        var existingJob = await jobs.GetByIdAsync(id);
        if (existingJob is null)
        {
            return NotFound();
        }

        if (!TryNormalizeCritical(request.Critical, out var critical, out var criticalError))
        {
            return criticalError!;
        }

        existingJob.Description = request.Description.Trim();
        existingJob.Condition = request.Condition?.Trim();
        existingJob.Miles = request.Miles;
        existingJob.Critical = critical;
        existingJob.Registration = request.Registration?.Trim();
        existingJob.Make = request.Make?.Trim();
        existingJob.Model = request.Model?.Trim();
        existingJob.CustomerName = request.CustomerName?.Trim();
        existingJob.AssignedTo = request.AssignedTo?.Trim();

        var updatedJob = await jobs.UpdateAsync(existingJob);
        if (updatedJob is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(updatedJob));
    }

    private static bool TryNormalizeCritical(string? critical, out string? normalized, out ActionResult? error)
    {
        normalized = null;
        error = null;

        if (string.IsNullOrWhiteSpace(critical))
        {
            return true;
        }

        normalized = AllowedCriticalLevels.FirstOrDefault(level =>
            level.Equals(critical.Trim(), StringComparison.OrdinalIgnoreCase));

        if (normalized is null)
        {
            error = new BadRequestObjectResult("Critical must be High, Medium, or Low.");
            return false;
        }

        return true;
    }

    private static Job MapRequestToJob(CreateJobRequest request, string? critical) => new()
    {
        Description = request.Description.Trim(),
        Condition = request.Condition?.Trim(),
        Miles = request.Miles,
        Critical = critical,
        Registration = request.Registration?.Trim(),
        Make = request.Make?.Trim(),
        Model = request.Model?.Trim(),
        CustomerName = request.CustomerName?.Trim(),
        AssignedTo = request.AssignedTo?.Trim()
    };

    private static JobResponse ToResponse(Job job) => new()
    {
        Id = job.Id,
        Description = job.Description,
        Condition = job.Condition,
        Miles = job.Miles,
        Critical = job.Critical,
        Registration = job.Registration,
        Make = job.Make,
        Model = job.Model,
        CustomerName = job.CustomerName,
        AssignedTo = job.AssignedTo,
        Status = job.Status,
        CreatedAt = job.CreatedAt
    };
}
