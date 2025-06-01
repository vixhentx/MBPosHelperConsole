using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBPosHelperConsole
{
    public class CmdEnvironment
    {
        public GTPattern? Pattern {  get; set; }
        public char? Symbol { get; set; }
        public int BlockIndex { get; set; } = 0;
        public int FaceIndex { get; set; } = 0;
    }
}
