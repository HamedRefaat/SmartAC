using Theoremone.SmartAc.Application.AlertsWrapper.ModelsDtos;
using Theoremone.SmartAc.Application.DeviceWrapper.ModelsDTOs;

namespace Theoremone.SmartAc.Application.AlertsWrapper
{
    public static class AlertsExtenstions
    {
        public static IEnumerable<AlertDto> MergeBatchReadingsAlerts(this IEnumerable<AlertDto> sourc)
        {
            return sourc.OrderByDescending(alert => alert.SensoreCreateDateTime).GroupBy(alert => alert.AlertType).Select(g => g.First()).ToList();
        }

        public static IEnumerable<AlertDto> SelfResolveBatchReadingsAlerts(this IEnumerable<AlertDto> alerts, IEnumerable<AlertDto> resolvedAlerts)
        {
            foreach (var newAlert in alerts)
            {
                var resolvedAlertWithFutureDate = resolvedAlerts.FirstOrDefault(resolvedAlert => resolvedAlert.AlertType.Equals(newAlert.AlertType) && resolvedAlert.IsNewerThan(newAlert));
                if (resolvedAlertWithFutureDate is not null)
                {
                    newAlert.AlertResolve = Domain.Enums.AlertResolveState.Resolved;
                    newAlert.SensoreUpdateDateTime = resolvedAlertWithFutureDate.SensoreCreateDateTime;
                }
            }
            return alerts;
        }


        public static IEnumerable<AlertDto> MergeWith(this IEnumerable<AlertDto> alerts, IEnumerable<AlertDto> unResolvedSavedAlerts)
        {
            foreach (var newAlert in alerts)
            {
                var alertFromUnResolved = unResolvedSavedAlerts.FirstOrDefault(alert => alert.AlertType == newAlert.AlertType);
                if (alertFromUnResolved is not null)
                {
                    // Not Updated Data
                    newAlert.AlertId = alertFromUnResolved.AlertId;
                    newAlert.ServerCreateDateTime = alertFromUnResolved.ServerCreateDateTime;
                    newAlert.SensoreCreateDateTime = alertFromUnResolved.SensoreCreateDateTime;
                    newAlert.ViewState = alertFromUnResolved.ViewState;
                    newAlert.AlertResolve = alertFromUnResolved.AlertResolve;
                }
            }
            return alerts;
        }
        
        public static IEnumerable<AlertDto> MustReopenAlerts(this IEnumerable<AlertDto> alerts, IEnumerable<AlertDto> resolvedAlertsSinceTime)
        {
            foreach (var newAlert in alerts)
            {
                var alertFromUnResolved = resolvedAlertsSinceTime.FirstOrDefault(alert => alert.AlertType == newAlert.AlertType);
                if (alertFromUnResolved is not null)
                {
                    newAlert.AlertId = alertFromUnResolved.AlertId;
                    newAlert.ServerCreateDateTime = alertFromUnResolved.ServerCreateDateTime;
                    newAlert.SensoreCreateDateTime = alertFromUnResolved.SensoreCreateDateTime;
                    newAlert.ViewState = alertFromUnResolved.ViewState;
                }
            }
            return alerts;
        }
    }
}
