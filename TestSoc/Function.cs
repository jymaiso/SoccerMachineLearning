using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSoc
{

    public interface IFunction
    {
        double Y(int x);
        double Sum(int x);
    }

    public abstract class BaseFunction
    {
        protected Dictionary<int, double> _values = new Dictionary<int, double>();

        public double Sum(int x)
        {
            double sum = 0;
            for (int i = 1; i <= x; i++)
            {
                sum += Y(i);
            }

            return sum;
        }

        public abstract double Y(int x);
    }

    public class ConstantFunction : BaseFunction, IFunction
    {
        public override double Y(int x)
        {
            return 1;
        }
    }

    public class LinearFunction : BaseFunction, IFunction
    {
        private double p;

        public LinearFunction(double p)
        {
            this.p = p;
        }

        public override double Y(int x)
        {
            if (!_values.ContainsKey(x))
                _values.Add(x, p * x);

            return _values[x];
        }
    }

    public class LogFunction : BaseFunction, IFunction
    {
        public override double Y(int x)
        {
            if (!_values.ContainsKey(x))
                _values.Add(x, Math.Log(x + 1));

            return _values[x];
        }
    }

    public class PowFunction : BaseFunction, IFunction
    {
        private double p;

        public PowFunction()
        {
            // TODO: Complete member initialization
            this.p = 2;
        }

        public PowFunction(double p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }
        public override double Y(int x)
        {
            if (!_values.ContainsKey(x))
                _values.Add(x, Math.Pow(x, p));

            return _values[x];
        }
    }
}
