using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Application.Common.Interfaces.Repository
{
    public interface IDeviceReadingRepo: IReadableAsyncRepository<DeviceReading, int>, IUpdatableAsyncRepository<DeviceReading>
    {
    }
}
