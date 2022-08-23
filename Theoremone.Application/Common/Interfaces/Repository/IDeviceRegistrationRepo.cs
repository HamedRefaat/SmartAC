using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Application.Common.Interfaces.Repository
{


    public interface IDeviceRegistrationRepo: IReadableAsyncRepository<DeviceRegistration, int>, IUpdatableAsyncRepository<DeviceRegistration>
    {
        Task DeactivateDeviceRegistrations(string serialNumber);
        Task<bool> IsActiveRegisterdDeviceWithToken(string? deviceSerialNumber, string? tokenId);
    }
}
