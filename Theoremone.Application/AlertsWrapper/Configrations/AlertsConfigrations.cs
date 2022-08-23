namespace Theoremone.SmartAc.Application.AlertsWrapper.Configrations
{
    public class AlertsConfigrations
    {
        public const string Section = "Alerts";
        public Temperature Temperature { get; set; } = new();
        public Humidity Humidity { get; set; } = new();
        public CarbonMonoxide CarbonMonoxide { get; set; } = new();
        public Health Health { get; set; } = new();
        public int ReopenAlertThresholdInMinutes { get; set; }

    }
    public class Temperature : MinMaxConfig
    {

    }
    public class Humidity : MinMaxConfig
    {

    }
    public class CarbonMonoxide : MinMaxConfig
    {
        public decimal Threshold { get; set; }
    }

    public class Health
    {
        public string Accepted { get; set; } = string.Empty;
    }
    public class MinMaxConfig
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
