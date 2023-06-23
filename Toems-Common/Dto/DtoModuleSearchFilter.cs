namespace Toems_Common.Dto
{
    public class DtoModuleSearchFilter
    {
        public int Limit { get; set; }
        public string Searchstring { get; set; }
        public bool IncludePrinter { get; set; }
        public bool IncludeSoftware { get; set; }
        public bool IncludeCommand { get; set; }
        public bool IncludeFileCopy { get; set; }
        public bool IncludeScript { get; set; }
        public bool IncludeWu { get; set; }
        public bool IncludeMessage { get; set; }
        public bool IncludeWinPe { get; set; }
        public bool IncludeWinget { get; set; }
        public bool IncludeUnassigned { get; set; }
    }
}
