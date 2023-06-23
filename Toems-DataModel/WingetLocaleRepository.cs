using System.Collections.Generic;
using System.Linq;
using Toems_Common;
using Toems_Common.Dto;
using Toems_Common.Entity;
using Toems_Common.Enum;

namespace Toems_DataModel
{
    public class WingetLocaleRepository : GenericRepository<EntityWingetLocaleManifest>
    {
        private readonly ToemsDbContext _context;

        public WingetLocaleRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

      
    }
}