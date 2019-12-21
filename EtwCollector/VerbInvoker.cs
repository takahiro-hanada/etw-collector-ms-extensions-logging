using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using CommandLine;
using EtwCollector.Properties;

namespace EtwCollector
{
    sealed class VerbInvoker
    {
        public static void Invoke(string[] args) => Parser.Default
            .ParseArguments(args.Select(arg => arg.Trim()).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToArray(), Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Select(type => new { type, verb = type.GetCustomAttribute<VerbAttribute>() })
                .Where(o => o.verb != null)
                .OrderBy(o => o.verb.Name)
                .Select(o => o.type)
                .ToArray()
                )
            .WithParsed<VerbBase>(verb =>
            {
                try
                {
                    verb.RunAsync().Wait();
                }
                catch (AggregateException ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    if (verb.Verbose)
                    {
                        Console.WriteLine(ex);
                    }
                    else
                    {
                        Console.WriteLine(Resources.ErrorOccurred, ex.InnerExceptions.Count);
                    }

                    Console.ResetColor();
                }
            })
            .WithNotParsed(errors => Environment.ExitCode = -1)
            ;
    }
}
