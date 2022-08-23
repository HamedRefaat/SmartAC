using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs;
using Theoremone.SmartAc.Domain.Entities;
using Theoremone.SmartAc.Domain.Enums;

namespace Theoremone.SmartAc.Api.Models;

public record DeviceReadingRecord(
        DateTimeOffset RecordedDateTime,
        decimal Temperature,
        decimal Humidity,
        decimal CarbonMonoxide,
        DeviceHealth Health)
{
    public DeviceReadingDto ToDeviceReading(string serialNumber, DateTimeOffset receivedDate)
    {
        return new()
        {
            DeviceSerialNumber = serialNumber,
            RecordedDateTime = RecordedDateTime,
            ReceivedDateTime = receivedDate,
            Temperature = Temperature,
            Humidity = Humidity,
            CarbonMonoxide = CarbonMonoxide,
            Health = Health,
        };
    }
}
