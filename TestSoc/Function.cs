using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSoc
{

    public interface IFunction
    {
        double Y(double x);
    }

    public class ConstantFunction : IFunction
    {
        public double Y(double x)
        {
            return 1;
        }
    }

    public class LinearFunction : IFunction
    {
        private int p;

        public LinearFunction(int p)
        {
            this.p = p;
        }
        public double Y(double x)
        {
            return p * x;
        }
    }

    public class LogFunction : IFunction
    {
        public double Y(double x)
        {
            return Math.Log(x + 1);
        }
    }

    public class ExpFunction : IFunction
    {
        public double Y(double x)
        {
            return Math.Pow(x, 3);
        }
    }
}
