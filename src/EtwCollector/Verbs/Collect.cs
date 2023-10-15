using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using EtwCollector.Properties;
using EtwStream;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;

namespace EtwCollector.Verbs
{
    [Verb("collect", HelpText = nameof(Resources.Collect_HelpText), ResourceType = typeof(Resources))]
    sealed class Collect : VerbBase
    {
        const string ProviderName = "Microsoft-Extensions-Logging";

        const string FormattedMessageEventName = "FormattedMessage";

        [Option('f', "filters", Separator = ',')]
        public IEnumerable<string> Filters { get; set; }

        [Option('l', "level", Default = LogLevel.Trace, HelpText = nameof(Resources.Collect_Level_HelpText), ResourceType = typeof(Resources))]
        public LogLevel Level { get; set; }

        [Option("screen.off")]
        public bool ScreenOff { get; set; }

        [Option("screen.timestamp-format", Default = "HH:mm:ss.fff")]
        public string TimeStampFormat { get; set; }

        [Option("csv")]
        public bool Csv { get; set; }

        [Option("csv.prefix")]
        public string CsvFileNamePrefix { get; set; }

        [Option("csv.folder")]
        public string CsvFolderName { get; set; }

        [Option("csv.encoding", Default = "UTF-8")]
        public string CsvEncodingName { get; set; }

        public override async Task RunAsync()
        {
            await Task.Yield();

            var csvPath = $"{CsvFileNamePrefix?.TrimEnd()}{(string.IsNullOrWhiteSpace(CsvFileNamePrefix) ? "" : "_")}{DateTime.Now:yy-MM-dd_HH-mm-ss}";
            csvPath = Path.ChangeExtension(csvPath, "csv");
            csvPath = Path.Combine(CsvFolderName ?? string.Empty, csvPath);
            var csvEncoding = Encoding.GetEncoding(CsvEncodingName);

            if (Csv)
            {
                var csvHeaderLine = string.Join(",", new[]
                {
                    "TimeStamp",
                    "MSec",
                    "PID",
                    "TID",
                    "Level",
                    "LoggerName",
                    "EventId",
                    "FormattedMessage",
                    "EventName",
                }.Select(v => "\"" + v + "\""));

                File.AppendAllLines(csvPath, new[] { csvHeaderLine }, csvEncoding);
            }

            object payloadByNameOrNull(TraceEvent traceEvent, string payloadName)
            {
                try
                {
                    return traceEvent.PayloadByName(payloadName);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
            }

            using (ObservableEventListener
                .FromTraceEvent(ProviderName)
                //.Do(o => Console.WriteLine(o.Dump()))
                //.Do(o => Console.WriteLine(o.DumpPayload()))
                //.Do(o => Console.WriteLine(o.DumpPayloadOrMessage()))
                //.Do(o => Console.WriteLine(o.ToJson()))
                .Do(o => Debug.WriteLine(o.ToString()))
                .Where(traceEvent => traceEvent.EventName == FormattedMessageEventName)
                .Select(traceEvent => new
                {
                    traceEvent,
                    payloads = traceEvent
                        .PayloadNames
                        .Select(payloadName => new { payloadName, payloadValue = payloadByNameOrNull(traceEvent, payloadName) })
                        .Where(payload => payload.payloadValue != null)
                        .ToDictionary(o => o.payloadName, o => o.payloadValue)
                })
                .Select(o => new
                {
                    o.traceEvent,
                    level = o.payloads.TryGetValue("Level", out object oLevel) && oLevel is int level ? (LogLevel)level : default(LogLevel?),
                    loggerName = o.payloads.TryGetValue("LoggerName", out object oLoggerName) && oLoggerName is string loggerName ? loggerName : default,
                    eventId = o.payloads.TryGetValue("EventId", out object oEventId) && oEventId is int eventId ? eventId : default(int?),
                    formattedMessage = o.payloads.TryGetValue("FormattedMessage", out object oFormattedMessage) && oFormattedMessage is string formattedMessage ? formattedMessage : default,
                    eventName = o.payloads.TryGetValue("EventName", out object oEvenName) && oEvenName is string eventName ? eventName : default,
                })
                .Where(o => o.level.HasValue)
                .Where(o => o.loggerName != null)
                .Where(o => !Filters.Any() || Filters.Any(filter => o.loggerName.StartsWith(filter)))
                .Where(o => o.level.Value >= Level)
                .Subscribe(o =>
                {
                    if (!ScreenOff)
                    {
                        var (label, color) = GetLogLevelOutput(o.level.Value);
                        Console.ForegroundColor = color ?? Console.ForegroundColor;
                        Console.Write($"{label}:");
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write($" {o.traceEvent.TimeStamp.ToString(TimeStampFormat)}");
                        Console.ResetColor();
                        Console.WriteLine($" {o.loggerName}{(o.eventId is null ? null : "/")}{o.eventId}{(o.eventName is null ? null : ":")}{o.eventName}");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine(string.Empty.PadLeft(6) + o.formattedMessage?.Replace(Environment.NewLine, Environment.NewLine + string.Empty.PadLeft(6)));
                        Console.ResetColor();
                        Console.WriteLine();
                    }

                    if (Csv)
                    {
                        var csvLine = string.Join(",", new object[]
                        {
                            $"{o.traceEvent.TimeStamp}",
                            $"{o.traceEvent.TimeStampRelativeMSec}",
                            $"{o.traceEvent.ProcessID}",
                            $"{o.traceEvent.ThreadID}",
                            $"{o.level}",
                            o.loggerName,
                            o.eventId,
                            o.formattedMessage,
                            o.eventName,
                        }.Select(v => "\"" + v + "\""));

                        File.AppendAllLines(csvPath, new[] { csvLine }, csvEncoding);
                    }
                }))
            {
                Console.WriteLine(Resources.PressEscKeyToStop);

                while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
            }
        }

        static (string, ConsoleColor?) GetLogLevelOutput(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace: return ("trce", ConsoleColor.Gray);
                case LogLevel.Debug: return ("dbug", null);
                case LogLevel.Information: return ("info", ConsoleColor.DarkGreen);
                case LogLevel.Warning: return ("warn", ConsoleColor.DarkYellow);
                case LogLevel.Error: return ("fail", ConsoleColor.DarkRed);
                case LogLevel.Critical: return ("crit", ConsoleColor.DarkMagenta);
                default: return ("????", null);
            }
        }
    }
}
