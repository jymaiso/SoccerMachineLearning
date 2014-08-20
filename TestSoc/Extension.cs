using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSoc
{
    public static class Ext
    {
        public static String TS(this double num)
        {
            return num.ToString().Replace(",", ".");
        }
    }
}
