using System.ComponentModel.DataAnnotations;

namespace GarageManagement.Api.DTOs;

public class CreateJobRequest
{
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Registration { get; set; }

    [MaxLength(100)]
    public string? Make { get; set; }

    [MaxLength(100)]
    public string? Model { get; set; }

    [MaxLength(200)]
    public string? CustomerName { get; set; }

    [MaxLength(200)]
    public string? AssignedTo { get; set; }
}
