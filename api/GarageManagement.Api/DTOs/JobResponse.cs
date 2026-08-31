namespace GarageManagement.Api.DTOs;

public class JobResponse
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Condition { get; set; }
    public int? Miles { get; set; }
    public string? Critical { get; set; }
    public string? Registration { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? CustomerName { get; set; }
    public string? AssignedTo { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
