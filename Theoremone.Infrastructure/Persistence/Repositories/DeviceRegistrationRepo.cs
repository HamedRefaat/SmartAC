using Theoremone.SmartAc.Application.Common.Interfaces.Repository;
using Theoremone.SmartAc.Domain.Entities;
using Theoremone.SmartAc.Infrastructure.Data.Extensions;
using Theoremone.SmartAc.Infrastructure.Persistence.Repositories.Base;

namespace Theoremone.SmartAc.Infrastructure.Persistence.Repositories
{
    public class DeviceRegistrationRepo: BaseRepository<DeviceRegistration, int>, IDeviceRegistrationRepo
    {
        public DeviceRegistrationRepo(SmartAcContext context) : base(context)
        {
        }

        public async Task DeactivateDeviceRegistrations(string serialNumber)
        {
            await AsQueryable().DeactivateRegistrations(serialNumber);
        }

        public async Task<bool> IsActiveRegisterdDeviceWithToken(string? deviceSerialNumber, string? tokenId)
        {
            return await AnyAsync(registration => registration.DeviceSerialNumber == deviceSerialNumber
            && registration.TokenId == tokenId
            && registration.Active);
        }
    }
}
