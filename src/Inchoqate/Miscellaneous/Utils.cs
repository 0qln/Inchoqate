using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miscellaneous
{
    public class Utils
    {
        public static double Lerp(double min, double max, double t)
        {
            return min + (max - min) * t;
        }
    }
}
