using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.Gamer.DevConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            NitroBolt.CommandLine.CommandLineManager.Process(args, typeof(Program).Assembly);
        }
    }
}
