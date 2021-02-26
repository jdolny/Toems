using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToemsUI.Client.Enums;

namespace ToemsUI.Client.Dtos
{
    public class ArchiveDeleteDialogDto
    {
        public bool IsYes { get; set; }

        public ActionType ActionType { get; set; }
    }
}
