using GarageManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageManagement.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Job> Jobs => Set<Job>();
}
