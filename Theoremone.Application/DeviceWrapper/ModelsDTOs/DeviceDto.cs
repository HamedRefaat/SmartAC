using Theoremone.SmartAc.Application.Mapper;
using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs
{
    public class DeviceDto : IMapFrom<Device>
    {
        
        public string SerialNumber { get; set; } = string.Empty;
        public string SharedSecret { get; set; } = string.Empty;
        public string FirmwareVersion { get; set; } = string.Empty;
        public DateTimeOffset? FirstRegistrationDate { get; set; }
        public DateTimeOffset? LastRegistrationDate { get; set; }
    }
}
