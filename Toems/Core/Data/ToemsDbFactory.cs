using Microsoft.EntityFrameworkCore;

namespace Toems_ServiceCore.Data;

public class ToemsDbFactory(IDbContextFactory<ToemsDbContext> factory) : IToemsDbFactory
{
    private bool _disposed;
    
    
    public async Task<ToemsDbContext> CreateDbContextAsync()
    {
        var context = await factory.CreateDbContextAsync();
        return context;
    }
}