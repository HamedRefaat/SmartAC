using Theoremone.SmartAc.Application.AlertsWrapper.Configrations;
using Theoremone.SmartAc.Application.AlertsWrapper.ModelsDtos;
using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs;
using Theoremone.SmartAc.Domain.Enums;

namespace Theoremone.SmartAc.Application.AlertsWrapper.Handelers
{
    public abstract class MainHandler
    {


        public virtual string WarningMessageTempalte { get; } = string.Empty;
        public virtual List<AlertDto> GetNewAlerts() => new();

        public virtual List<AlertDto> GetResolvedAlerts() => new();
        public virtual AlertDto GerateNewAlert(DeviceReadingDto dr, AlertType alertType, string message)
        {
            return new()
            {
                AlertType = alertType,
                DeviceSerialNumber = dr.DeviceSerialNumber,
                Message = message,
                SensoreCreateDateTime = dr.RecordedDateTime,
                SensoreUpdateDateTime = dr.RecordedDateTime,

            };
        }

        public virtual AlertDto GerateResolvedAlerts(DeviceReadingDto dr, AlertType alertType)
        {
            return new()
            {
                AlertType = alertType,
                DeviceSerialNumber = dr.DeviceSerialNumber,
                SensoreCreateDateTime = dr.RecordedDateTime,
                AlertResolve = AlertResolveState.Resolved,

            };
        }
    }
}
