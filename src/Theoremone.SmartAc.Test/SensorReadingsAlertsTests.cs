using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Theoremone.SmartAc.Api.Controllers;
using Theoremone.SmartAc.Api.Models;
using Theoremone.SmartAc.Domain.Enums;
using Theoremone.SmartAc.Infrastructure.Persistence;
using Theoremone.SmartAc.Test.Infrastructure;

namespace Theoremone.SmartAc.Test;

public class SensorReadingsAlertsTests : IDisposable
{
    private readonly SmartAcApplication<DeviceIngestionController> _application;
    private readonly HttpClient _client;
    private readonly SmartAcContext _smartAcContext;

    public SensorReadingsAlertsTests()
    {
        _application = new SmartAcApplication<DeviceIngestionController>();
        _client = _application.CreateClient();
        _smartAcContext = _application.Services.CreateScope().ServiceProvider
            .GetRequiredService<SmartAcContext>();
    }

    public void Dispose()
    {
        _client.Dispose();
        _application.Dispose();
    }

    [Theory]
    [InlineData("test-ABC-123-XYZ-001", "secret-ABC-123-XYZ-001", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-002", "secret-ABC-123-XYZ-002", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-003", "secret-ABC-123-XYZ-003", "1.0.0")]
    public async Task OutOfRangeTemp_Should_Add_OutOfRangeTemp_Alert(string serialNumber, string secret, string firmware)
    {
        var deviceToken = await RegisterDeviceForToken(serialNumber, secret, firmware);
        var testData = GenerateDummyData_OutOfRangeTemp();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, deviceToken);

        var response = await _client
            .PostAsJsonAsync($"/api/v1/device/readings/batch", testData);

        var alerts = await _smartAcContext.Alerts.Where(i => i.DeviceSerialNumber == serialNumber && i.AlertType == AlertType.OutOfRangeTemp).ToListAsync();

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Single(alerts);

        foreach (var alert in alerts)
        {
            Assert.Equal(AlertViewState.unviewed, alert.ViewState);
            Assert.Equal(AlertResolveState.New, alert.AlertResolve);
            Assert.Equal($"Sensor Temperature reported out of range Value.", alert.Message);

        }
    }

    [Theory]
    [InlineData("test-ABC-123-XYZ-001", "secret-ABC-123-XYZ-001", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-002", "secret-ABC-123-XYZ-002", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-003", "secret-ABC-123-XYZ-003", "1.0.0")]
    public async Task OutOfRangeHumidity_Should_Add_OutOfRange_humidity_Alert(string serialNumber, string secret, string firmware)
    {
        var deviceToken = await RegisterDeviceForToken(serialNumber, secret, firmware);
        var testData = GenerateDummyData_OutOfRangeHumidity();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, deviceToken);

        var response = await _client
            .PostAsJsonAsync($"/api/v1/device/readings/batch", testData);

        var alerts = await _smartAcContext.Alerts.Where(i => i.DeviceSerialNumber == serialNumber && i.AlertType == AlertType.OutOfRangeHumidity).ToListAsync();

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Single(alerts);

        foreach (var alert in alerts)
        {
            Assert.Equal(AlertViewState.unviewed, alert.ViewState);
            Assert.Equal(AlertResolveState.New, alert.AlertResolve);
            Assert.Equal($"Sensor Humidity reported out of range Value.", alert.Message);

        }
    }


    [Theory]
    [InlineData("test-ABC-123-XYZ-001", "secret-ABC-123-XYZ-001", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-002", "secret-ABC-123-XYZ-002", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-003", "secret-ABC-123-XYZ-003", "1.0.0")]
    public async Task OutOfRangeCarbonMonoxide_Should_Add_OutOfRange_CarbonMonoxide_Alert(string serialNumber, string secret, string firmware)
    {
        var deviceToken = await RegisterDeviceForToken(serialNumber, secret, firmware);
        var testData = GenerateDummyData_OutOfRangeCO();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, deviceToken);

        var response = await _client
            .PostAsJsonAsync($"/api/v1/device/readings/batch", testData);

        var alerts = await _smartAcContext.Alerts.Where(i => i.DeviceSerialNumber == serialNumber && i.AlertType == AlertType.OutOfRangeCO).ToListAsync();

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Single(alerts);

        foreach (var alert in alerts)
        {
            Assert.Equal(AlertViewState.unviewed, alert.ViewState);
            Assert.Equal(AlertResolveState.New, alert.AlertResolve);
            Assert.Equal($"Sensor CarbonMonoxide reported out of range Value.", alert.Message);

        }
    }


    [Theory]
    [InlineData("test-ABC-123-XYZ-001", "secret-ABC-123-XYZ-001", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-002", "secret-ABC-123-XYZ-002", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-003", "secret-ABC-123-XYZ-003", "1.0.0")]
    public async Task DangerousCoLevels_Should_Add_DangerousCoLeve_CarbonMonoxide_Alert(string serialNumber, string secret, string firmware)
    {
        var deviceToken = await RegisterDeviceForToken(serialNumber, secret, firmware);
        var testData = GenerateDummyData_DangetousCO();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, deviceToken);

        var response = await _client
            .PostAsJsonAsync($"/api/v1/device/readings/batch", testData);

        var alerts = await _smartAcContext.Alerts.Where(i => i.DeviceSerialNumber == serialNumber && i.AlertType == AlertType.DangerousCO).ToListAsync();

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Single(alerts);

        foreach (var alert in alerts)
        {
            Assert.Equal(AlertViewState.unviewed, alert.ViewState);
            Assert.Equal(AlertResolveState.New, alert.AlertResolve);
            Assert.Equal($"CarbonMonoxide value has exceeded danger limit.", alert.Message);

        }
    }


    [Theory]
    [InlineData("test-ABC-123-XYZ-001", "secret-ABC-123-XYZ-001", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-002", "secret-ABC-123-XYZ-002", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-003", "secret-ABC-123-XYZ-003", "1.0.0")]
    public async Task PoorHealth_Should_Add_PoorHealthHandler_Health_Alert(string serialNumber, string secret, string firmware)
    {
        var deviceToken = await RegisterDeviceForToken(serialNumber, secret, firmware);
        var testData = GenerateDummyData_PoorHealth();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, deviceToken);

        var response = await _client
            .PostAsJsonAsync($"/api/v1/device/readings/batch", testData);

        var alerts = await _smartAcContext.Alerts.Where(i => i.DeviceSerialNumber == serialNumber && i.AlertType == AlertType.PoorHealth)
         .OrderBy(s => s.SensoreCreateDateTime)
            .ToListAsync();
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Single(alerts);
        var testdatCausedAllert = testData.Where(i => i.Health != DeviceHealth.Ok).OrderByDescending(s => s.RecordedDateTime).ToList();
        foreach (var (sentData, alert) in Enumerable.Zip(testdatCausedAllert, alerts))
        {
            Assert.Equal(AlertViewState.unviewed, alert.ViewState);
            Assert.Equal(AlertResolveState.New, alert.AlertResolve);
            Assert.Equal($"Device is reporting health problem: {sentData.Health}.", alert.Message);
            Assert.Equal(sentData.RecordedDateTime, alert.SensoreCreateDateTime);

        }
    }


    [Theory]
    [InlineData("test-ABC-123-XYZ-001", "secret-ABC-123-XYZ-001", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-002", "secret-ABC-123-XYZ-002", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-003", "secret-ABC-123-XYZ-003", "1.0.0")]
    public async Task ResolvedAlertsInTheSameBatchShouldCreateAlertWithStatusResolved(string serialNumber, string secret, string firmware)
    {
        var deviceToken = await RegisterDeviceForToken(serialNumber, secret, firmware);
        var testData = GenerateDummyData_SelfResolvedInBatch();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, deviceToken);

        var response = await _client
            .PostAsJsonAsync($"/api/v1/device/readings/batch", testData);

        var alerts = await _smartAcContext.Alerts.Where(i => i.DeviceSerialNumber == serialNumber)
         .OrderBy(s => s.SensoreCreateDateTime)
            .ToListAsync();
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Equal(5,alerts.Count);
      
        foreach (var alert in alerts)
        {
            Assert.Equal(AlertViewState.unviewed, alert.ViewState);
            Assert.Equal(AlertResolveState.Resolved, alert.AlertResolve);
            Assert.NotEqual(alert.SensoreCreateDateTime, alert.SensoreUpdateDateTime);
            Assert.True(alert.SensoreUpdateDateTime > alert.SensoreCreateDateTime);
        }
    }

    [Theory]
    [InlineData("test-ABC-123-XYZ-001", "secret-ABC-123-XYZ-001", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-002", "secret-ABC-123-XYZ-002", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-003", "secret-ABC-123-XYZ-003", "1.0.0")]
    public async Task ResolvedAlertsInTheSameBatchShouldCreateAlertWithStatusResolvedWithUpdateDate(string serialNumber, string secret, string firmware)
    {
        var deviceToken = await RegisterDeviceForToken(serialNumber, secret, firmware);
        var testData = GenerateDummyData_SelfResolvedInBatch_OutOfRangeTemp();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, deviceToken);

        var response = await _client
            .PostAsJsonAsync($"/api/v1/device/readings/batch", testData);

        var alerts = await _smartAcContext.Alerts.Where(i => i.DeviceSerialNumber == serialNumber)
         .OrderBy(s => s.SensoreCreateDateTime)
            .ToListAsync();
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Single(alerts);

        var test = testData.OrderByDescending(i => i.RecordedDateTime).First();

        foreach (var alert in alerts)
        {
            Assert.Equal(AlertViewState.unviewed, alert.ViewState);
            Assert.Equal(AlertResolveState.Resolved, alert.AlertResolve);
            Assert.NotEqual(alert.SensoreCreateDateTime, alert.SensoreUpdateDateTime);
            Assert.True(alert.SensoreUpdateDateTime > alert.SensoreCreateDateTime);
            Assert.Equal(alert.SensoreUpdateDateTime, test.RecordedDateTime);


        }
    }


    [Theory]
    [InlineData("test-ABC-123-XYZ-001", "secret-ABC-123-XYZ-001", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-002", "secret-ABC-123-XYZ-002", "1.0.0")]
    [InlineData("test-ABC-123-XYZ-003", "secret-ABC-123-XYZ-003", "1.0.0")]
    public async Task OutOfRangeTemp_Should_Add_Duplicate_should_Merge(string serialNumber, string secret, string firmware)
    {
        var deviceToken = await RegisterDeviceForToken(serialNumber, secret, firmware);
        var testData = GenerateDummyData_OutOfRangeTemp();
        
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, deviceToken);

        var response = await _client
            .PostAsJsonAsync($"/api/v1/device/readings/batch", testData);
        var nextData = GenerateDummyData_OutOfRangeTemp_next();

        var responsenext = await _client
            .PostAsJsonAsync($"/api/v1/device/readings/batch", nextData);

        var alerts = await _smartAcContext.Alerts.Where(i => i.DeviceSerialNumber == serialNumber && i.AlertType == AlertType.OutOfRangeTemp).ToListAsync();

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, responsenext.StatusCode);
        Assert.Single(alerts);

        foreach (var alert in alerts)
        {
            Assert.Equal(AlertViewState.unviewed, alert.ViewState);
            Assert.Equal(AlertResolveState.New, alert.AlertResolve);
            Assert.Equal($"Sensor Temperature reported out of range Value.", alert.Message);

        }
    }


    #region Helpers and Data Genrators
    private async Task<string> RegisterDeviceForToken(string serialNumber, string secret, string firmware)
    {
        _client.DefaultRequestHeaders.Add("x-device-shared-secret", secret);

        var response = await _client
            .PostAsync($"/api/v1/device/{serialNumber}/register?firmwareVersion={firmware}", default);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return await response.Content.ReadAsStringAsync();
    }

    public static List<DeviceReadingRecord> GenerateDummyData_PoorHealth()
    {
        return new List<DeviceReadingRecord>()
        {
            // normal
            new(DateTimeOffset.UtcNow.AddMinutes(-40), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),

            //PoorHealth
              new(DateTimeOffset.UtcNow.AddMinutes(-20), 25.0m, 73.99m, 3.22m, DeviceHealth.NeedFilter),
              new(DateTimeOffset.UtcNow.AddMinutes(-10), 25.0m, 73.99m, 3.22m, DeviceHealth.NeedService),
        };
    }

    public static List<DeviceReadingRecord> GenerateDummyData_OutOfRangeTemp()
    {
        return new List<DeviceReadingRecord>()
        {
            

           

            // Noraml Temp
             new(DateTimeOffset.UtcNow.AddMinutes(-40), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),

              // out of range Temp
             new(DateTimeOffset.UtcNow.AddMinutes(-20), -40.0m, 73.99m, 3.22m, DeviceHealth.Ok),
              new(DateTimeOffset.UtcNow.AddMinutes(-10), 125.0m, 73.99m, 3.22m, DeviceHealth.Ok),

        };
    }

    public static List<DeviceReadingRecord> GenerateDummyData_OutOfRangeTemp_next()
    {
        return new List<DeviceReadingRecord>()
        {
            

           

            // Noraml Temp
             new(DateTimeOffset.UtcNow.AddMinutes(-20), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),

              // out of range Temp
             new(DateTimeOffset.UtcNow.AddMinutes(-10), -40.0m, 73.99m, 3.22m, DeviceHealth.Ok),
              new(DateTimeOffset.UtcNow.AddMinutes(-5), 125.0m, 73.99m, 3.22m, DeviceHealth.Ok),

        };
    }

    public static List<DeviceReadingRecord> GenerateDummyData_OutOfRangeHumidity()
    {
        return new List<DeviceReadingRecord>()
        {

            // normal
              new(DateTimeOffset.UtcNow.AddMinutes(-40), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),
            // out of range humidity
             new(DateTimeOffset.UtcNow.AddMinutes(-20), 25.0m, -73.99m, 3.22m, DeviceHealth.Ok),
              new(DateTimeOffset.UtcNow.AddMinutes(-10), 25.0m, 173.99m, 3.22m, DeviceHealth.Ok),

        };
    }

    public static List<DeviceReadingRecord> GenerateDummyData_OutOfRangeCO()
    {
        return new List<DeviceReadingRecord>()
        {
            
            // normal
              new(DateTimeOffset.UtcNow.AddMinutes(-40), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),

            // out of range CarbonMonoxide
            new(DateTimeOffset.UtcNow.AddMinutes(-20), 25.0m, 73.99m, -3.22m, DeviceHealth.Ok),
             new(DateTimeOffset.UtcNow.AddMinutes(-10), 25.0m, 73.99m, 113.22m, DeviceHealth.Ok),

        };
    }

    public static List<DeviceReadingRecord> GenerateDummyData_DangetousCO()
    {
        return new List<DeviceReadingRecord>()
        {
            
            // normal
              new(DateTimeOffset.UtcNow.AddMinutes(-40), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),

            // DangetousCO CarbonMonoxide
            new(DateTimeOffset.UtcNow.AddMinutes(-20), 25.0m, 73.99m, 9.22m, DeviceHealth.Ok),
             new(DateTimeOffset.UtcNow.AddMinutes(-10), 25.0m, 73.99m, 10.22m, DeviceHealth.Ok),

        };
    }

    public static List<DeviceReadingRecord> GenerateDummyData_SelfResolvedInBatch()
    {
        return new List<DeviceReadingRecord>()
        {
             // DangetousCO CarbonMonoxide
            new(DateTimeOffset.UtcNow.AddMinutes(-20), 25.0m, 73.99m, 9.22m, DeviceHealth.Ok),
            // normal
            new(DateTimeOffset.UtcNow.AddMinutes(-10), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),

           
             // out of range CarbonMonoxide
            new(DateTimeOffset.UtcNow.AddMinutes(-20), 25.0m, 73.99m, -3.22m, DeviceHealth.Ok),
            // normal
            new(DateTimeOffset.UtcNow.AddMinutes(-15), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),


            // out of range humidity
             new(DateTimeOffset.UtcNow.AddMinutes(-20), 25.0m, -73.99m, 3.22m, DeviceHealth.Ok),
               // normal
             new(DateTimeOffset.UtcNow.AddMinutes(-16), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),


              // out of range Temp
             new(DateTimeOffset.UtcNow.AddMinutes(-20), -40.0m, 73.99m, 3.22m, DeviceHealth.Ok),
              // Noraml Temp
             new(DateTimeOffset.UtcNow.AddMinutes(-5), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),



             //PoorHealth
              new(DateTimeOffset.UtcNow.AddMinutes(-20), 25.0m, 73.99m, 3.22m, DeviceHealth.NeedFilter),
              // normal
              new(DateTimeOffset.UtcNow.AddMinutes(-3), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),

            



        };
    }

    public static List<DeviceReadingRecord> GenerateDummyData_SelfResolvedInBatch_OutOfRangeTemp()
    {
        return new List<DeviceReadingRecord>()
        {
            


              // out of range Temp
             new(DateTimeOffset.UtcNow.AddMinutes(-20), -40.0m, 73.99m, 3.22m, DeviceHealth.Ok),
              // Noraml Temp
             new(DateTimeOffset.UtcNow.AddMinutes(-5), 25.0m, 73.99m, 3.22m, DeviceHealth.Ok),

        };
    }
    #endregion

}
