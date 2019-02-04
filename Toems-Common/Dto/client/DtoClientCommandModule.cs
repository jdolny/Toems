using System.Collections.Generic;

namespace Toems_Common.Dto.client
{
    public class DtoClientCommandModule
    {
        public DtoClientCommandModule()
        {
            Files = new List<DtoClientFileHash>();
            SuccessCodes = new List<string>();
            RunAs = string.Empty;
        }

        public string Guid { get; set; }
        public string DisplayName { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
        public int Order { get; set; }
        public int Timeout { get; set; }
        public bool RedirectOutput { get; set; }
        public bool RedirectError { get; set; }
        public List<string> SuccessCodes { get; set; }
        public List<DtoClientFileHash> Files { get; set; }
        public string WorkingDirectory { get; set; }
        public string RunAs { get; set; }
        
    }
}
