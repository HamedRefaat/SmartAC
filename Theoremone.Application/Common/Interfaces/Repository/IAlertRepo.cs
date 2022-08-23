using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Application.Common.Interfaces.Repository
{
  public  interface IDeviceRepo : IReadableAsyncRepository<Device,string>, IUpdatableAsyncRepository<Device>
    {
        public Task<Device?> GetDeviceBySerialNumber(string serialNumber, string sharedSecret);
        void UpdateDeviceDetails(Device deviceEntity, string firmwareVersion, DeviceRegistration newRegistrationDevice);
    }
}
