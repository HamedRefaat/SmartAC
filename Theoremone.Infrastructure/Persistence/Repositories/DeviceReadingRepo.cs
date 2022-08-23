using Theoremone.SmartAc.Application.Common.Interfaces.Repository;
using Theoremone.SmartAc.Domain.Entities;
using Theoremone.SmartAc.Infrastructure.Persistence.Repositories.Base;

namespace Theoremone.SmartAc.Infrastructure.Persistence.Repositories
{
    public class DeviceReadingRepo: BaseRepository<DeviceReading, int>, IDeviceReadingRepo
    {
        public DeviceReadingRepo(SmartAcContext context) : base(context)
        {

        }
    
    }
    
}
