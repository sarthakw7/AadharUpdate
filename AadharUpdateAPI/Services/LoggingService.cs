using Serilog;
using Serilog.Events;

namespace AadharUpdateAPI.Services
{
    public static class LoggingService
    {
        public static void ConfigureLogging()
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        public static void LogError(Exception ex, string message)
        {
            Log.Error(ex, message);
        }

        public static void LogInformation(string message)
        {
            Log.Information(message);
        }

        public static void LogWarning(string message)
        {
            Log.Warning(message);
        }

        public static void LogDebug(string message)
        {
            Log.Debug(message);
        }
    }
} 