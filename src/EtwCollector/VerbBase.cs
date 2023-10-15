using System.Threading.Tasks;
using CommandLine;
using EtwCollector.Properties;

namespace EtwCollector
{
    abstract class VerbBase
    {
        [Option('v', "verbose", HelpText = nameof(Resources.VerbBase_Verbose), ResourceType = typeof(Resources))]
        public bool Verbose { get; set; }

        public abstract Task RunAsync();
    }
}
