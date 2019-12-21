using System.Threading.Tasks;
using CommandLine;
using EtwStream;
using EtwCollector.Properties;

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
