using System.Collections.Generic;
using System.Linq;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;
using Toems_ServiceCore.Data;

namespace Toems_DataModel
{
    public class WingetLocaleRepository(ToemsDbContext _context) : GenericRepository<EntityWingetLocaleManifest>(_context)
    {


      
    }
}