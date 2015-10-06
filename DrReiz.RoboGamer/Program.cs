using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.RoboGamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.FirstOrDefault() == "--test-mouse-client")
            {
                using (var client = new MouseClient("169.254.200.106"))
                {
                    client.MouseEvent(MouseEventFlags.MOVE, 100, 100);
                }
                return;
            }

            Zuma.Execute();
        }
    }
}
