using Microsoft.Extensions.Logging;
using Windows.UI.Xaml.Controls;

namespace SampleLogger
{
    sealed partial class MainPage : Page
    {
        readonly ILogger _logger;

        public MainPage()
        {
            InitializeComponent();

            _logger = App.MyLoggerFactory.CreateLogger<MainPage>();
        }

        void LogCritical()
        {
            var eventId = EventIdToggle.IsOn ? EventNameToggle.IsOn ?
                new EventId(1, "testEventName") :
                new EventId(1) :
                default(EventId?);

            if (eventId.HasValue)
            {
                if (ArgsToggle.IsOn)
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
                if (ArgsToggle.IsOn)
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
