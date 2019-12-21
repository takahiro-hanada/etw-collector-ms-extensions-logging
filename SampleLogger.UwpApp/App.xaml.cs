using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SampleLogger
{
    sealed partial class App : Application
    {
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
