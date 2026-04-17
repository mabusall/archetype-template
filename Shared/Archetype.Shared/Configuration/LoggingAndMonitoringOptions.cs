namespace Archetype.Shared.Configuration;

public class LoggingAndMonitoringOptions
{
    public static string ConfigurationKey => "LoggingAndMonitoring";

    public bool IsEnabled { get; set; }
    public required string Environment { get; set; }
    public required ElasticsearchOptions Elasticsearch { get; set; }
    public required ApmOptions Apm { get; set; }
    public required OpenTelemetryOptions OpenTelemetry { get; set; }
    public required LoggingOptions Logging { get; set; }
}

public class LoggingOptions
{
    public required string Project { get; set; }
    public required string Application { get; set; }
    public required string FolderPath { get; set; }

    // Serilog sink controls
    public bool EnableConsoleLogs { get; set; } = true;
    public bool EnableFileLogs { get; set; } = true;
}

public class ElasticsearchOptions
{
    public bool IsEnabled { get; set; } = true;
    public required string AddressUri { get; set; }
    public string? ApiKey { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class ApmOptions
{
    public required string AddressUri { get; set; }
    public required string ServiceName { get; set; }
    public required string ServiceVersion { get; set; }
    public string CaptureBody { get; set; } = "all";
    public bool CaptureHeaders { get; set; } = true;
    public double TransactionSampleRate { get; set; } = 1.0;
}

public class OpenTelemetryOptions
{
    public required string AddressUri { get; set; }
    public bool EnableTracing { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
    public string TraceSampler { get; set; } = "AlwaysOn";
}