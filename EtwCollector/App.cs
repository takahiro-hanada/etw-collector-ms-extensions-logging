using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;

namespace EtwCollector
{
    static class App
    {
        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                args = new[] { "--help" };

                while (args?.Any(o => !string.IsNullOrWhiteSpace(o)) == true)
                {
                    VerbInvoker.Invoke(args);

                    Console.Write('>');

                    args = Console.ReadLine().Split(' ');
                }
            }
            else
            {
                if (args?.Any(o => !string.IsNullOrWhiteSpace(o)) == true)
                {
                    VerbInvoker.Invoke(args);
                }
            }
        }
    }
}
