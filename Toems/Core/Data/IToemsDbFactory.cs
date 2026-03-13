namespace Toems_ServiceCore.Data;

public interface IToemsDbFactory
{
    Task<ToemsDbContext> CreateDbContextAsync();
}