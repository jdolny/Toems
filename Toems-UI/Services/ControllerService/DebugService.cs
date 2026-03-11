using Toems_ServiceCore.Data;

namespace Toems_UI.Services.ControllerService;

public class DebugService
{
    private readonly IToemsDbFactory _toemsDbFactory;

    public DebugService(IToemsDbFactory toemsDbFactory)
    {
        _toemsDbFactory = toemsDbFactory;
    }

    public async Task<string?> VerifyDb()
    {
        await using var db = await _toemsDbFactory.CreateDbContextAsync();
        var comp = db.Computers.FirstOrDefault();
        return comp?.Name;
    }
}