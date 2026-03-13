using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_ServiceCore.Data;
using Toems_ServiceCore.Infrastructure;

namespace Toems_UI.Services.ControllerService;


public class ComputerService(IToemsDbFactory toemsDbFactory)
{

    public async Task<List<EntityComputer>> SearchComputers(DtoComputerFilter filter)
    {
        await using var sparcDb = await toemsDbFactory.CreateDbContextAsync();
        
        var c = sparcDb.Computers.ToList();
        return sparcDb.Computers.ToList();
    }
}

