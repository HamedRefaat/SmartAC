using Theoremone.SmartAc.Application.AlertsWrapper.Configrations;
using Theoremone.SmartAc.Application.AlertsWrapper.ModelsDtos;
using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs;
using Theoremone.SmartAc.Domain.Enums;

namespace Theoremone.SmartAc.Application.AlertsWrapper.Handelers
{
    public class DangerousCoLevelsHadler: MainHandler
    {
        public override string WarningMessageTempalte => "{0} value has exceeded danger limit.";

        private readonly IEnumerable<DeviceReadingDto> _deviceReading;
        private readonly AlertsConfigrations _alertsConfigrations;
        public DangerousCoLevelsHadler(IEnumerable<DeviceReadingDto> deviceReadings, AlertsConfigrations alertsConfigrations)
        {
            _deviceReading = deviceReadings;
            _alertsConfigrations = alertsConfigrations;
        }

        public override List<AlertDto> GetNewAlerts()
        {
            return _deviceReading.Where(d => DeviceInDagerCo(d,_alertsConfigrations.CarbonMonoxide))
                .Select(d => base.GerateNewAlert(d, AlertType.DangerousCO, string.Format(WarningMessageTempalte, nameof(d.CarbonMonoxide))))
                .ToList();
        }

        public override List<AlertDto> GetResolvedAlerts()
        {
            return _deviceReading.Where(d => !DeviceInDagerCo(d,_alertsConfigrations.CarbonMonoxide))
                .Select(d => base.GerateResolvedAlerts(d,AlertType.DangerousCO))
                .ToList();
        }

        private bool DeviceInDagerCo(DeviceReadingDto d, CarbonMonoxide carbonMonoxide)
        {
            return d.CarbonMonoxide > carbonMonoxide.Threshold;
        }
    }
}
