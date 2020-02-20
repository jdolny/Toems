using System.Collections.Generic;
namespace Toems_Common.Dto.clientimaging
{
    public class WinPEProfileList
    {
        public string Count { get; set; }
        public string FirstProfileId { get; set; }
        public List<WinPEProfile> ImageProfiles { get; set; }
    }
}