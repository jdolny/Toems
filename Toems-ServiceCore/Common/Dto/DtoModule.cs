using Toems_Common.Enum;

namespace Toems_Common.Dto
{
    public class DtoModule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
        public EnumModule.ModuleType ModuleType { get; set; }
        
        public string DropZoneIdentifier { get; set; }
    }
}
