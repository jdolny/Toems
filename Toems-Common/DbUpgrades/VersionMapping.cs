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
            _mapping.Add(132, "1.3.2.0");
            _mapping.Add(133, "1.3.3.0");
            _mapping.Add(144, "1.4.4.0");
            _mapping.Add(145, "1.4.5.0");
            _mapping.Add(146, "1.4.6.0");
            _mapping.Add(147, "1.4.7.0");
            _mapping.Add(148, "1.4.8.0");
            _mapping.Add(150, "1.5.0.0");


        }

        public Dictionary<int, string> Get()
        {
            return _mapping;
        }
    }
}
