using System.IO;

namespace Toems_Common.Dto
{
    public class DtoFileUpload
    {
        public string Filename { get; set; }
        public Stream InputStream { get; set; }
        public int PartIndex { get; set; }
        public int TotalParts { get; set; }
        public string OriginalFilename { get; set; }
        public string PartUuid { get; set; }
        public ulong FileSize { get; set; }
        public string UploadMethod { get; set; }
        public string DestinationDirectory { get; set; }
        public string ModuleGuid { get; set; }
        public string AttachmentGuid { get; set; }
        public string AssetId { get; set; }
        public string ComputerId { get; set; }
        public string Username { get; set; }
        
    }
}
