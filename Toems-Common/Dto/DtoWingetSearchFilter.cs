namespace Toems_Common.Dto
{
    public class DtoWingetSearchFilter
    {
        public int Limit { get; set; }
        public string Searchstring { get; set; }
        public bool IncludePackageIdentifier { get; set; }
        public bool IncludePackageName { get; set; }
        public bool IncludePublisher { get; set; }
        public bool IncludeTags { get; set; }
        public bool IncludeMoniker { get; set; }
        public bool ExactMatch { get; set; }
        public bool LatestVersionOnly { get; set; }

    
    }
}
