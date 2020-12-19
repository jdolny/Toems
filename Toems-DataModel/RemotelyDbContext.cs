using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Entity.Remotely;

namespace Toems_DataModel
{
    public class RemotelyDbContext : DbContext
    {
        public RemotelyDbContext()
            : base(
                new SQLiteConnection
                {
                    ConnectionString =
                        string.Format(@"data source={0}\Theopenem\Remotely\Remotely.db",
                            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles))
                }, true)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Organization> Organizations { get; set; }

    }
}
