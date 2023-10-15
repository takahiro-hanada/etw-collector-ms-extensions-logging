using System;
using System.Linq;
using System.Reactive.Linq;

namespace EtwCollector
{
    static class App
    {
        static void Main(string[] args)
        {
            if (args?.Any(o => !string.IsNullOrWhiteSpace(o)) == true)
            {
                VerbInvoker.Invoke(args);
            }
            else
            {
                args = new[] { "--help" };

                while (args?.Any(o => !string.IsNullOrWhiteSpace(o)) == true)
                {
                    VerbInvoker.Invoke(args);

                    Console.Write('>');

                    args = Console.ReadLine().Split(' ');
                }
            }
        }
    }
}
