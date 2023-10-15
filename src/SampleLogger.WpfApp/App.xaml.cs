using System.Windows;
using Microsoft.Extensions.Logging;

namespace SampleLogger.WpfApp
{
    public partial class App : Application
    {
        public static ILoggerFactory MyLoggerFactory { get; } = LoggerFactory.Create(builder => builder
            .AddDebug()
            .AddEventSourceLogger()
            );

    }
}
