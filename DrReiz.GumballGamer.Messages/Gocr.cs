using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DrReiz.GumballGamer.Messages
{
    public interface IGocr : Orleans.IGrainWithStringKey
    {
        Task<Shot> PerceptionText(string path);
    }

    public partial class Shot
    {
        public readonly string shotName;
        public readonly Area[] areas;
    }
    public partial class Area
    {
        public readonly int x;
        public readonly int y;
        public readonly int width;
        public readonly int height;
        public readonly string name;
        public readonly string value;
    }
}
