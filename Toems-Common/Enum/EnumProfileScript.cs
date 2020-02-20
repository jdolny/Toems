using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Enum
{
    public class EnumProfileScript
    {
        public enum RunWhen
        {
            Disabled = 0,
            BeforeImaging = 1,
            BeforeFileCopy = 2,
            AfterFileCopy = 3,
            
        }
    }
}
