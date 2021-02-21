using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace WSBC.DiscordBot
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
                    config.ReadFrom.Configuration(configuration, _sectionName);
                // otherwise use defaults
                else
                {
                    config.WriteTo.Console()
                        .MinimumLevel.Is(Debugger.IsAttached ? LogEventLevel.Verbose : LogEventLevel.Information)
                        .Enrich.FromLogContext();
                }

                // create the logger
                Serilog.Log.Logger = config.CreateLogger();

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
                Serilog.Log.Fatal((Exception)e.ExceptionObject, "An exception was unhandled");
                Serilog.Log.CloseAndFlush();
            }
            catch { }
        }
        #endregion

        #region DISCORD.NET SUPPORT
        public static void Log(this ILogger logger, LogMessage message)
        {
            LogLevel level = message.Severity.ToLogLevel();
            if (!logger.IsEnabled(level))
                return;

            using (logger.BeginScope(new Dictionary<string, object>()
            {
                { "Source", $"DiscordNet: {message.Source}" }
            }))
            {
                logger.Log(level, message.Exception, message.Message);
            }
        }

        public static IDisposable BeginCommandScope(this ILogger log, SocketCommandContext context, object module = null, [CallerMemberName] string cmdName = null)
            => BeginCommandScope(log, context, module?.GetType(), cmdName);

        public static IDisposable BeginCommandScope(this ILogger log, SocketCommandContext context, Type moduleType = null, [CallerMemberName] string cmdName = null)
        {
            Dictionary<string, object> state = new Dictionary<string, object>
            {
                { "Command.UserID", context.User?.Id },
                { "Command.MessageID", context.Message?.Id },
                { "Command.ChannelID", context.Channel?.Id },
                { "Command.GuildID", context.Guild?.Id }
            };
            if (!string.IsNullOrWhiteSpace(cmdName))
                state.Add("Command.Method", cmdName);
            if (moduleType != null)
                state.Add("Command.Module", moduleType.Name);
            return log.BeginScope(state);
        }

        public static LogLevel ToLogLevel(this LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Critical:
                    return LogLevel.Critical;
                case LogSeverity.Error:
                    return LogLevel.Error;
                case LogSeverity.Warning:
                    return LogLevel.Warning;
                case LogSeverity.Info:
                    return LogLevel.Information;
                case LogSeverity.Debug:
                    return LogLevel.Debug;
                case LogSeverity.Verbose:
                    return LogLevel.Trace; ;
                default:
                    throw new ArgumentException($"Unknown severity {severity}", nameof(severity));
            }
        }

        public static LogSeverity ToLogSeverity(this LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Critical:
                    return LogSeverity.Critical;
                case LogLevel.Error:
                    return LogSeverity.Error;
                case LogLevel.Warning:
                    return LogSeverity.Warning;
                case LogLevel.Information:
                    return LogSeverity.Info;
                case LogLevel.Debug:
                    return LogSeverity.Debug;
                case LogLevel.Trace:
                    return LogSeverity.Verbose;
                default:
                    throw new ArgumentException($"Unknown log level {level}", nameof(level));
            }
        }
        #endregion
    }
}
