using Microsoft.Extensions.Logging;
using Windows.UI.Xaml.Controls;

namespace SampleLogger
{
    sealed partial class MainPage : Page
    {
#pragma warning disable CS0618
        ILogger Logger { get; } = new LoggerFactory()
            .AddEventSourceLogger()
            .AddDebug()
            .CreateLogger<MainPage>();
#pragma warning restore CS0618

        public MainPage()
        {
            InitializeComponent();
        }

        void LogCritical()
        {
            Logger.LogCritical("test");
        }
    }
}
