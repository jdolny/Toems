using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toems_Common.Enum
{
    public class EnumTaskStatus
    {
        public enum ImagingStatus
        {
            TaskCreated = 0,
            WaitingForLogin = 1,
            CheckedIn = 2,
            InImagingQueue = 3,
            Imaging = 4
            
        }
    }
}
