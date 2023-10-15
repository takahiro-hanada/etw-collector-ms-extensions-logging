using System.Threading.Tasks;
using CommandLine;
using EtwCollector.Properties;
using EtwStream;

namespace EtwCollector.Verbs
{
    [Verb("clear", HelpText = nameof(Resources.ClearSession_HelpText), ResourceType = typeof(Resources))]
    sealed class Clear : VerbBase
    {
        public override async Task RunAsync()
        {
            await Task.Yield();

            ObservableEventListener.ClearAllActiveObservableEventListenerSession();
        }
    }
}
