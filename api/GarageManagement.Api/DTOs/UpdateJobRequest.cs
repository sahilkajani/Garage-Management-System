using System.ComponentModel.DataAnnotations;

namespace GarageManagement.Api.DTOs;

public class UpdateJobRequest
{
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Condition { get; set; }

    [Range(0, 9_999_999)]
    public int? Miles { get; set; }

    [MaxLength(20)]
    public string? Critical { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Unscheduled";

    public DateTime? ScheduledDate { get; set; }

    public DateTime? CompletedDate { get; set; }

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
