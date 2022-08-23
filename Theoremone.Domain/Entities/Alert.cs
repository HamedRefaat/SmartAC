namespace Theoremone.SmartAc.Domain.Entities
{
    public class Alert : BaseEntity
    {
        public int AlertId { get; set; }
        public string DeviceSerialNumber { get; set; } = string.Empty;
        public AlertType AlertType { get; set; }
        public DateTimeOffset ServerCreateDateTime { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset SensoreCreateDateTime { get; set; }
        public DateTimeOffset SensoreUpdateDateTime { get; set; }
        public string Message { get; set; } = string.Empty;
        public AlertViewState ViewState { get; set; } = AlertViewState.unviewed;
        public AlertResolveState AlertResolve { get; set; } = AlertResolveState.New;


    }
}
