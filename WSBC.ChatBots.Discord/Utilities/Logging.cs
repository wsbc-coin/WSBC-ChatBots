using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using WSBC.ChatBots.Discord.Utilities;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace WSBC.ChatBots.Discord
{
    static class Logging
    {
        #region INITIALIZATION
        private static bool _initialized = false;
        private static readonly object _lock = new object();
        private const string _sectionName = "Logging";

        public static void ConfigureLogging(IConfiguration configuration)
        {
            lock (_lock)
            {
                LoggerConfiguration config = new LoggerConfiguration();

                // load from config if section exists
                if (configuration.GetSection(_sectionName).Exists())
                {
                    config.ReadFrom.Configuration(configuration, _sectionName);
                    // configure datadog
                    DatadogOptions ddOptions = configuration.GetSection(_sectionName).GetSection("DataDog").Get<DatadogOptions>();
                    if (!string.IsNullOrWhiteSpace(ddOptions?.ApiKey))
                    {
                        config.WriteTo.DatadogLogs(
                            ddOptions.ApiKey,
                            source: ".NET",
                            service: ddOptions.ServiceName ?? "WsbcDiscordBot",
                            host: ddOptions.HostName ?? Environment.MachineName,
                            new string[] { },
                            ddOptions.ToDatadogConfiguration(),
                            // no need for debug logs in datadag
                            logLevel: ddOptions.OverrideLogLevel ?? LogEventLevel.Information
                        );
                    }
                }
                // otherwise use defaults
                else
                {
                    config.WriteTo.Console()
                        .MinimumLevel.Is(Debugger.IsAttached ? LogEventLevel.Verbose : LogEventLevel.Information)
                        .Enrich.FromLogContext();
                }


                // create the logger
                Log.Logger = config.CreateLogger();

                // enable logging of unhandled exceptions, but only when initializing for the first time
                if (!_initialized)
                    AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                _initialized = true;
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Log.Fatal((Exception)e.ExceptionObject, "An exception was unhandled");
                Log.CloseAndFlush();
            }
            catch { }
        }
        #endregion

        #region EXCEPTIONS LOGGING
        /// <summary>Logs exception with critical log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsCritical(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogCritical(exception, message, args);
            return true;
        }
        /// <summary>Logs exception with error log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsError(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogError(exception, message, args);
            return true;
        }
        /// <summary>Logs exception with warning log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsWarning(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogWarning(exception, message, args);
            return true;
        }
        /// <summary>Logs exception with information log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsInformation(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogInformation(exception, message, args);
            return true;
        }
        /// <summary>Logs exception with debug log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsDebug(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogDebug(exception, message, args);
            return true;
        }
        /// <summary>Logs exception with trace log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsTrace(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogTrace(exception, message, args);
            return true;
        }
        #endregion
    }
}
