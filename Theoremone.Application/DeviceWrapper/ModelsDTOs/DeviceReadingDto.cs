using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs;
using Theoremone.SmartAc.Application.Mapper;
using Theoremone.SmartAc.Domain.Entities;
using Theoremone.SmartAc.Domain.Enums;

namespace Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs
{
    public class DeviceReadingDto:IMapFrom<DeviceReading>
    {
        public int DeviceReadingId { get; set; }

     
        public decimal Temperature { get; set; }

       
        public decimal Humidity { get; set; }

      
        public decimal CarbonMonoxide { get; set; }

        public DeviceHealth Health { get; set; }
        public DateTimeOffset RecordedDateTime { get; set; }
        public DateTimeOffset ReceivedDateTime { get; set; } = DateTime.UtcNow;

        public string DeviceSerialNumber { get; set; } = string.Empty;
        public DeviceDto Device { get; set; } = null!;
    }
}
