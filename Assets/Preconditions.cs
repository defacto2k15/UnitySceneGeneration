using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    static class Preconditions
    {
        public static void Assert( bool valueToCheck, string failureMessage ){
            if (!valueToCheck)
            {
                throw new ArgumentException(failureMessage);
            }
        }
    }
}
