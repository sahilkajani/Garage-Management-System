using GarageManagement.Api.Models;

namespace GarageManagement.Api.Data;

public interface IJobRepository
{
    Task<IReadOnlyList<Job>> GetAllAsync();
    Task<Job?> GetByIdAsync(int id);
    Task<Job> CreateAsync(Job job);
}
