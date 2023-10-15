using Microsoft.Extensions.Logging;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SampleLogger
{
    sealed partial class App : Application
    {
        public static ILoggerFactory MyLoggerFactory { get; } = LoggerFactory.Create(builder => builder
            .AddDebug()
            .AddEventSourceLogger()
            );

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = new Frame();

            Window.Current.Content = rootFrame;

            rootFrame.Navigate(typeof(MainPage));

            Window.Current.Activate();
        }
    }
}
