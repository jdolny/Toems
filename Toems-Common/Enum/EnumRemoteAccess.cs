using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Enum
{
    public class EnumRemoteAccess
    {
        public enum RaStatus
        {
            NotConfigured = 0,
            UsersCreated = 1,
            MeshCreated = 2,
            Configured = 3
        }
    }
}
