using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.DbUpgrades
{
    public class VersionMapping
    {
        private readonly Dictionary<int, string> _mapping;

        public VersionMapping()
        {
            _mapping = new Dictionary<int, string>();
            _mapping.Add(121, "1.2.1.0");
            _mapping.Add(130, "1.3.0.0");
            _mapping.Add(131, "1.3.1.0");

        }

        public Dictionary<int, string> Get()
        {
            return _mapping;
        }
    }
}
