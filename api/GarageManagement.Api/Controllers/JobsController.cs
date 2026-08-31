using GarageManagement.Api.Data;
using GarageManagement.Api.DTOs;
using GarageManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GarageManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController(IJobRepository jobs) : ControllerBase
{
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
        var job = new Job
        {
            Description = request.Description.Trim(),
            Registration = request.Registration?.Trim(),
            Make = request.Make?.Trim(),
            Model = request.Model?.Trim(),
            CustomerName = request.CustomerName?.Trim(),
            AssignedTo = request.AssignedTo?.Trim(),
            Status = "Unassigned",
            CreatedAt = DateTime.UtcNow
        };

        var createdJob = await jobs.CreateAsync(job);

        return CreatedAtAction(nameof(GetJob), new { id = createdJob.Id }, ToResponse(createdJob));
    }

    private static JobResponse ToResponse(Job job) => new()
    {
        Id = job.Id,
        Description = job.Description,
        Registration = job.Registration,
        Make = job.Make,
        Model = job.Model,
        CustomerName = job.CustomerName,
        AssignedTo = job.AssignedTo,
        Status = job.Status,
        CreatedAt = job.CreatedAt
    };
}
