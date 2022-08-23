﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Infrastructure.Persistence;

/// <summary>
/// Smart AC Db context targeting Sqlite provider
/// </summary>
public class SmartAcContext : DbContext
{
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceRegistration> DeviceRegistrations => Set<DeviceRegistration>();
    public DbSet<DeviceReading> DeviceReadings => Set<DeviceReading>();

    public DbSet<Alert> Alerts => Set<Alert>();

    public SmartAcContext(DbContextOptions<SmartAcContext> options) : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetToStringConverter>(); // SqlLite workaround for DateTimeOffset sorting
    }
}