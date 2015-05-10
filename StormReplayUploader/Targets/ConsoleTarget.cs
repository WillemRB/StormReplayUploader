using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormReplayUploader.Targets
{
    public class ConsoleTarget : IStormReplayTarget
    {
        public void Notify(string path)
        {
            Console.WriteLine(path);
        }
    }
}
