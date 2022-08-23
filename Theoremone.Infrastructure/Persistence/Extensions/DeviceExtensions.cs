using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Infrastructure.Data.Extensions;

public static class DeviceExtensions
{
    public static Device UpdateDeviceDetails(this Device device, string firmwareVersion, DeviceRegistration deviceRegistration)
    {
        device.FirmwareVersion = firmwareVersion;
        device.FirstRegistrationDate ??= deviceRegistration.RegistrationDate;
        device.LastRegistrationDate = deviceRegistration.RegistrationDate;

        return device;
    }
}
