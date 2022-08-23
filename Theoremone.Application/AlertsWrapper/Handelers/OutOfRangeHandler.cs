using Theoremone.SmartAc.Application.AlertsWrapper.Configrations;
using Theoremone.SmartAc.Application.AlertsWrapper.ModelsDtos;
using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs;
using Theoremone.SmartAc.Domain.Enums;

namespace Theoremone.SmartAc.Application.AlertsWrapper.Handelers
{
    public class OutOfRangeHandler : MainHandler
    {

        private readonly IEnumerable<DeviceReadingDto> _deviceReading;
        private readonly AlertsConfigrations _alertsConfigrations;
        public OutOfRangeHandler(IEnumerable<DeviceReadingDto> deviceReadings, AlertsConfigrations alertsConfigrations)
        {
            _deviceReading = deviceReadings;
            _alertsConfigrations = alertsConfigrations;
        }

        public override string WarningMessageTempalte => "Sensor {0} reported out of range Value.";
        public override List<AlertDto> GetNewAlerts()
        {
            var alerts = new List<AlertDto>();
            alerts.AddRange(GetOutOfRangeAlerts(_deviceReading, AlertType.OutOfRangeTemp,nameof(DeviceReadingDto.Temperature), dr => OutOfRangeTemperature(_alertsConfigrations.Temperature, dr)));
            alerts.AddRange(GetOutOfRangeAlerts(_deviceReading, AlertType.OutOfRangeHumidity,nameof(DeviceReadingDto.Humidity), dr => OutOfRangeHumididty(_alertsConfigrations.Humidity, dr)));
            alerts.AddRange(GetOutOfRangeAlerts(_deviceReading, AlertType.OutOfRangeCO, nameof(DeviceReadingDto.CarbonMonoxide), dr => OutOfRangeCarbonMonxide(_alertsConfigrations.CarbonMonoxide, dr)));

            return alerts;
        }

        public override List<AlertDto> GetResolvedAlerts()
        {
            var alerts = new List<AlertDto>();
            alerts.AddRange(GetResolvedAlerts(_deviceReading, AlertType.OutOfRangeTemp,  dr => !OutOfRangeTemperature(_alertsConfigrations.Temperature, dr)));
            alerts.AddRange(GetResolvedAlerts(_deviceReading, AlertType.OutOfRangeHumidity, dr => !OutOfRangeHumididty(_alertsConfigrations.Humidity, dr)));
            alerts.AddRange(GetResolvedAlerts(_deviceReading, AlertType.OutOfRangeCO, dr => !OutOfRangeCarbonMonxide(_alertsConfigrations.CarbonMonoxide, dr)));

            return alerts;
        }

        private List<AlertDto> GetOutOfRangeAlerts(IEnumerable<DeviceReadingDto> deviceReadingDtos, AlertType alertType, string SensorName, Func<DeviceReadingDto, bool> predict)
        {
            return deviceReadingDtos.Where(predict)
                .Select(dr => base.GerateNewAlert(dr, alertType: alertType, string.Format(WarningMessageTempalte, SensorName))).ToList();
        }

        private List<AlertDto> GetResolvedAlerts(IEnumerable<DeviceReadingDto> deviceReadingDtos, AlertType alertType, Func<DeviceReadingDto, bool> predict)
        {
            return deviceReadingDtos.Where(predict)
                .Select(dr => base.GerateResolvedAlerts(dr, alertType: alertType)).ToList();
        }

        private static bool OutOfRangeHumididty(Humidity humidity, DeviceReadingDto dr)
        {
            return dr.Humidity < humidity.Min || dr.Humidity > humidity.Max;
        }

        private static bool OutOfRangeCarbonMonxide(CarbonMonoxide carbonMonoxide, DeviceReadingDto dr)
        {
            return dr.CarbonMonoxide < carbonMonoxide.Min || dr.CarbonMonoxide > carbonMonoxide.Max;
        }

        private static bool OutOfRangeTemperature(Temperature temperature, DeviceReadingDto dr)
        {
            return dr.Temperature < temperature.Min || dr.Temperature > temperature.Max;
        }
    }
}
