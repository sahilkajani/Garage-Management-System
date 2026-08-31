using GarageManagement.Api.Data;
using GarageManagement.Api.DTOs;
using GarageManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarageManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobResponse>>> GetJobs()
    {
        var jobs = await db.Jobs
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => ToResponse(j))
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobResponse>> GetJob(int id)
    {
        var job = await db.Jobs.FindAsync(id);
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

        db.Jobs.Add(job);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, ToResponse(job));
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
