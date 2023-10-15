using System.Windows;
using Microsoft.Extensions.Logging;

namespace SampleLogger.WpfApp
{
    public partial class MainWindow : Window
    {
        readonly ILogger _logger = App.MyLoggerFactory.CreateLogger<MainWindow>();

        public MainWindow() => InitializeComponent();

        void Button_Click(object sender, RoutedEventArgs e)
        {
            var eventId = EventIdCheck.IsChecked == true ? EventNameCheck.IsChecked == true ?
                new EventId(1, "testEventName") :
                new EventId(1) :
                default(EventId?);

            if (eventId.HasValue)
            {
                if (ArgsCheck.IsChecked == true)
                {
                    _logger.LogCritical(eventId.Value, "testMessage {0}", "testArg");
                }
                else
                {
                    _logger.LogCritical(eventId.Value, "testMessage");
                }
            }
            else
            {
                if (ArgsCheck.IsChecked == true)
                {
                    _logger.LogCritical("testMessage {0}", "testArg");
                }
                else
                {
                    _logger.LogCritical("testMessage");
                }
            }
        }
    }
}
