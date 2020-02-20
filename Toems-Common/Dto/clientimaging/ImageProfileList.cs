using System.Collections.Generic;

namespace Toems_Common.Dto.clientimaging
{
    public class ImageProfileList
    {
        public string Count { get; set; }
        public string FirstProfileId { get; set; }
        public List<string> ImageProfiles { get; set; }
    }
}