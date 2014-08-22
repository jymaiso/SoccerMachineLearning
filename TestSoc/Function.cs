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
        private int p;

        public ExpFunction()
        {
            // TODO: Complete member initialization
            this.p = 2;
        }

        public ExpFunction(int p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }
        public double Y(double x)
        {
            return Math.Pow(x, p);
        }
    }
}
