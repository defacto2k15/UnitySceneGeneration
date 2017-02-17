using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    class MathHelp
    {
        public static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }
    }
}
