namespace GarageManagement.Api.DTOs;

public class JobResponse
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Registration { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? CustomerName { get; set; }
    public string? AssignedTo { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
