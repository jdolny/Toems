using Microsoft.EntityFrameworkCore;
using Toems_Common.Entity;

namespace Toems_ServiceCore.Data;

public class ToemsDbContext : DbContext
{
    public DbSet<EntityComputer> Computers { get; set; }
    
    public ToemsDbContext(DbContextOptions<ToemsDbContext> options)
        : base(options)
    {
    }
}