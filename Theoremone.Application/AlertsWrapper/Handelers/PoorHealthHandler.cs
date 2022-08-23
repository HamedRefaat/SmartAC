using Theoremone.SmartAc.Application.AlertsWrapper.Configrations;
using Theoremone.SmartAc.Application.AlertsWrapper.ModelsDtos;
using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs;
using Theoremone.SmartAc.Domain.Enums;

namespace Theoremone.SmartAc.Application.AlertsWrapper.Handelers
{
    public class PoorHealthHandler: MainHandler
    {
        public override string WarningMessageTempalte => "Device is reporting health problem: {0}.";
        private readonly IEnumerable<DeviceReadingDto> _deviceReading;
        private readonly AlertsConfigrations _alertsConfigrations;
        public PoorHealthHandler(IEnumerable<DeviceReadingDto> deviceReadings, AlertsConfigrations alertsConfigrations)
        {
            _deviceReading = deviceReadings;
            _alertsConfigrations = alertsConfigrations;
        }
        public override List<AlertDto> GetNewAlerts()
        {
            return _deviceReading.Where(dr => !AcceptedHealth(dr,_alertsConfigrations.Health))
                 .Select(dr => base.GerateNewAlert(dr, AlertType.PoorHealth, string.Format(WarningMessageTempalte, dr.Health.ToString()))).ToList();
        }

        public override List<AlertDto> GetResolvedAlerts()
        {
            return _deviceReading.Where(dr => AcceptedHealth(dr,_alertsConfigrations.Health))
                .Select(d => base.GerateResolvedAlerts(d, AlertType.PoorHealth))
                .ToList();
        }

        private bool AcceptedHealth(DeviceReadingDto dr, Health health)
        {
            return dr.Health.ToString().Equals(health.Accepted, StringComparison.OrdinalIgnoreCase);
        }
    }
}
