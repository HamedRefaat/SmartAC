using AutoMapper;
using Microsoft.Extensions.Logging;
using Theoremone.SmartAc.Application.AlertsWrapper;
using Theoremone.SmartAc.Application.Common.Interfaces.Repository;
using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTO;
using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs;
using Theoremone.SmartAc.Domain.Entities;
namespace Theoremone.SmartAc.Application.DeviceWrapper
{
    public class DeviceWrapper
    {
        private readonly IDeviceRepo _deviceRepo;
        private readonly IDeviceRegistrationRepo _deviceRegistrationRepo;
        private readonly IDeviceReadingRepo _deviceReadingRepo;
        private readonly DeviceReadingAlertHandler _alertHandler;
        private readonly ILogger<DeviceWrapper> _logger;
        private readonly IMapper _mapper;
      
        public DeviceWrapper(
            IDeviceRepo deviceRepo,
            IDeviceRegistrationRepo deviceRegistrationRepo,
            IDeviceReadingRepo deviceReadingRepo,
            DeviceReadingAlertHandler alertHandler,
            ILogger<DeviceWrapper> logger,
            IMapper mapper
           )
        {
            _deviceRepo = deviceRepo;
            _deviceRegistrationRepo = deviceRegistrationRepo;
            _deviceReadingRepo = deviceReadingRepo;
            _alertHandler= alertHandler;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task AddDeviceReadings(List<DeviceReadingDto> deviceReadings, string serialNumber)
        {
            await _deviceReadingRepo.AddMultibleAsync(_mapper.Map<IEnumerable<DeviceReading>>(deviceReadings));
            await _deviceRepo.SaveChangesAsync();
            await _alertHandler.Handel(deviceReadings, serialNumber);
            // TODO move this alert to hagfire to by Async
        }

        public async Task<DeviceDto> GetRegisterDeviceDevice(GetDeviceRegisterdQuery deviceRegisteration)
        {
            var device = await _deviceRepo.GetDeviceBySerialNumber(deviceRegisteration.serialNumber, deviceRegisteration.sharedSecret);

            if (device == null)
            {
                _logger.LogDebug("There is not a matching device for serial number {serialNumber} and the secret provided.", deviceRegisteration.serialNumber);
                throw new UnauthorizedAccessException("Something is wrong on the information provided, please review.");
            }
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task RegisterDevice(DeviceDto deviceDto, string tokenId, string firmwareVersion)
        {
            Device deviceEntity = _mapper.Map<Device>(deviceDto);
            var newRegistrationDevice = new DeviceRegistration()
            {
                DeviceSerialNumber = deviceDto.SerialNumber,
                TokenId = tokenId
            };
            await _deviceRegistrationRepo.DeactivateDeviceRegistrations(deviceDto.SerialNumber);
            await _deviceRegistrationRepo.AddAsync(newRegistrationDevice);
            _deviceRepo.UpdateDeviceDetails(deviceEntity, firmwareVersion, newRegistrationDevice);
            await _deviceRepo.SaveChangesAsync();

            _logger.LogDebug("A new registration record with tokenId \"{tokenId}\" has been created for the device \"{serialNumber}\"", deviceDto.SerialNumber, tokenId);

        }
    }
}
