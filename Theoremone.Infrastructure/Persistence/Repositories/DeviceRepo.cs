using Theoremone.SmartAc.Application.Common.Interfaces.Repository;
using Theoremone.SmartAc.Domain.Entities;
using Theoremone.SmartAc.Infrastructure.Data.Extensions;
using Theoremone.SmartAc.Infrastructure.Persistence.Repositories.Base;

namespace Theoremone.SmartAc.Infrastructure.Persistence.Repositories
{
    public class DeviceRepo : BaseRepository<Device, string>, IDeviceRepo
    {
        public DeviceRepo(SmartAcContext context) : base(context)
        {
        }

        public async Task<Device?> GetDeviceBySerialNumber(string serialNumber, string sharedSecret)
        {
            return await FiratOrDefault(d => d.SerialNumber == serialNumber && d.SharedSecret == sharedSecret);
        }

        public void UpdateDeviceDetails(Device deviceEntity, string firmwareVersion, DeviceRegistration newRegistrationDevice)
        {
             deviceEntity.UpdateDeviceDetails(firmwareVersion, newRegistrationDevice);
            Update(deviceEntity);
        }
    }
}
