using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Theoremone.SmartAc.Application.AlertsWrapper.Configrations;
using Theoremone.SmartAc.Application.AlertsWrapper.Handelers;
using Theoremone.SmartAc.Application.AlertsWrapper.ModelsDtos;
using Theoremone.SmartAc.Application.Common.Interfaces;
using Theoremone.SmartAc.Application.Common.Interfaces.Repository;
using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs;
using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Application.AlertsWrapper
{
    public class DeviceReadingAlertHandler
    {
        private readonly AlertsConfigrations _alertsConfigrations;
        private readonly IAlertRepo _alertRepo;
        private readonly IMapper _mapper;
        private IDateTime _dateTimeService;
        private readonly ILogger<DeviceReadingAlertHandler> _logger;
        public DeviceReadingAlertHandler(
            IOptions<AlertsConfigrations> alertsConfigrationsOptions,
            IAlertRepo alertRepo,
            IMapper mapper,
            IDateTime dateTime,
            ILogger<DeviceReadingAlertHandler> logger
            )
        {
            _alertsConfigrations = alertsConfigrationsOptions.Value;
            _alertRepo = alertRepo;
            _mapper = mapper;
            _dateTimeService = dateTime;
            _logger = logger;
        }

        public async Task Handel(IEnumerable<DeviceReadingDto> deviceReading, string serialNumber)
        {
            List<MainHandler> mainHandlers = new()
            {
                new OutOfRangeHandler(deviceReading, _alertsConfigrations),
                new PoorHealthHandler(deviceReading, _alertsConfigrations),
                new DangerousCoLevelsHadler(deviceReading, _alertsConfigrations)
            };

            var unResolvedAlertsFromCurrentBatch = mainHandlers.Select(handler => handler.GetNewAlerts()).SelectMany(alert => alert);
            var resolvedAlertsFromCurrentBatch = mainHandlers.Select(handler => handler.GetResolvedAlerts()).SelectMany(alert => alert);
            resolvedAlertsFromCurrentBatch = IgnoreUnResolvedWithHigterDate(resolvedAlertsFromCurrentBatch, unResolvedAlertsFromCurrentBatch);
            var unresolvedAlertsForDevice = await GetUnResolvedAlerts(serialNumber);
            var resolvedSinceConfigrableTime = await GetResolvedAlertsSience(serialNumber, _alertsConfigrations.ReopenAlertThresholdInMinutes);
            var autoResolvedAlerts = GetAutoResolvedAlerts(unresolvedAlertsForDevice, resolvedAlertsFromCurrentBatch);
            var stillUnResolved = unresolvedAlertsForDevice.Where(alert => !autoResolvedAlerts.Select(alert => alert.AlertId).Contains(alert.AlertId));
            var alertes = unResolvedAlertsFromCurrentBatch.MergeBatchReadingsAlerts().MergeWith(stillUnResolved).SelfResolveBatchReadingsAlerts(resolvedAlertsFromCurrentBatch);
            var newReportedAlertsInBatch = alertes.Where(alert => alert.AlertId == 0).MustReopenAlerts(resolvedSinceConfigrableTime);
            var newReportedAlertsInBatchNotExsitsInREsolved = newReportedAlertsInBatch.Where(alert => alert.AlertId == 0);
            var reopendAlerts = newReportedAlertsInBatch.Where(alert => alert.AlertId != 0);
            var mergedAlerts = alertes.Where(alert => alert.AlertId != 0);
            await SaveAlerts(autoResolvedAlerts, newReportedAlertsInBatch, newReportedAlertsInBatchNotExsitsInREsolved, reopendAlerts, mergedAlerts);
        }


        private async Task SaveAlerts(IEnumerable<AlertDto> autoResolvedAlerts, IEnumerable<AlertDto> newReportedAlertsInBatch, IEnumerable<AlertDto> newReportedAlertsInBatchNotExsitsInREsolved, IEnumerable<AlertDto> reopendAlerts, IEnumerable<AlertDto> mergedAlerts)
        {
            bool anyChecges = false;
            if (newReportedAlertsInBatch.Any())
            {
                await _alertRepo.AddMultibleAsync(_mapper.Map<List<Alert>>(newReportedAlertsInBatchNotExsitsInREsolved));
                anyChecges = true;
            }

            if (mergedAlerts.Any())
            {
                _alertRepo.UpdateMultible(_mapper.Map<List<Alert>>(mergedAlerts));
                anyChecges = true;
            }
            if (autoResolvedAlerts.Any())
            {
                _alertRepo.UpdateMultible(_mapper.Map<List<Alert>>(autoResolvedAlerts));
                anyChecges = true;
            }
            if (reopendAlerts.Any())
            {
                _alertRepo.UpdateMultible(_mapper.Map<List<Alert>>(reopendAlerts));
                anyChecges = true;
            }
            if (anyChecges)
                await _alertRepo.SaveChangesAsync();
        }
        private IEnumerable<AlertDto> IgnoreUnResolvedWithHigterDate(IEnumerable<AlertDto> resolvedAlertsFromCurrentBatch, IEnumerable<AlertDto> unResolvedAlertsFromCurrentBatch)
        {
            var resolved = new List<AlertDto>();
            foreach (var alert in resolvedAlertsFromCurrentBatch)
            {
                bool anyInUnresolvedWithHierDate = unResolvedAlertsFromCurrentBatch.Any(unResolvedAlert => unResolvedAlert.AlertType == alert.AlertType && unResolvedAlert.SensoreCreateDateTime > alert.SensoreCreateDateTime);
                if (!anyInUnresolvedWithHierDate)
                {
                    resolved.Add(alert);
                }
            }
            return resolved;
        }
        private IEnumerable<AlertDto> GetAutoResolvedAlerts(IEnumerable<AlertDto> unresolvedAlertsForDevice, IEnumerable<AlertDto> resolvedAlertsFromCurrentBatch)
        {
            var resolvedAlerts = new List<AlertDto>();
            foreach (var unResolvedAlert in unresolvedAlertsForDevice)
            {
                var resolved = resolvedAlertsFromCurrentBatch.FirstOrDefault(alert => alert.AlertType == unResolvedAlert.AlertType);
                if (resolved is not null)
                {
                    unResolvedAlert.AlertResolve = Domain.Enums.AlertResolveState.Resolved;
                    resolvedAlerts.Add(unResolvedAlert);
                }
            }
            return resolvedAlerts;

        }
        private async Task<IEnumerable<AlertDto>> GetUnResolvedAlerts(string serialNumber)
        {
            var alerts = await _alertRepo.GetWhereAsync(a => a.DeviceSerialNumber == serialNumber && a.AlertResolve != Domain.Enums.AlertResolveState.Resolved);
            return _mapper.Map<IEnumerable<AlertDto>>(alerts);
        }
        private async Task<IEnumerable<AlertDto>> GetResolvedAlertsSience(string serialNumber, int reopenAlertThresholdInMinutes)
        {

            var alerts = await _alertRepo.GetWhereAsync(a =>
            a.DeviceSerialNumber == serialNumber
            && a.AlertResolve == Domain.Enums.AlertResolveState.Resolved
            && a.SensoreUpdateDateTime >= _dateTimeService.Now.AddMinutes(reopenAlertThresholdInMinutes * -1)
            );
            return _mapper.Map<IEnumerable<AlertDto>>(alerts);
        }

    }
}
