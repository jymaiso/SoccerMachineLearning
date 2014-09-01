using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public static double ConvertToDouble(this string num)
        {
            return double.Parse(num.Replace(".", ","));
        }

        /// <summary>
        /// Return as string the name of a property giving a lambda expression
        /// </summary>
        public static string Prop<T, TValue>(Expression<Func<T, TValue>> memberAccess)
        {
            return ((MemberExpression)memberAccess.Body).Member.Name;
        }

        /// <summary>
        /// Shuffle a list
        /// </summary>
        public static List<T> Shuffle<T>(this IList<T> list)
        {
            return list.Shuffle(DateTime.Now.Millisecond * DateTime.Now.Minute);
        }

        /// <summary>
        /// Shuffle a list
        /// </summary>
        public static List<T> Shuffle<T>(this IList<T> list, Random rnd)
        {
            var newList = list.ToList();
            int n = newList.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = newList[k];
                newList[k] = newList[n];
                newList[n] = value;
            }

            return newList;
        }

        /// <summary>
        /// Shuffle a list
        /// </summary>
        public static List<T> Shuffle<T>(this IList<T> list, int seed)
        {
            Random rng = new Random(seed);
            return list.Shuffle(rng);
        }

        public static int IndexOfMax(this double[] list)
        {
            double maxValue = list.Max();
            return list.ToList().IndexOf(maxValue);
        }

        public static IEnumerable<T[]> Combinations<T>(this IList<T> argList, int argSetSize)
        {
            if (argList == null) throw new ArgumentNullException("argList");
            if (argSetSize <= 0) throw new ArgumentException("argSetSize Must be greater than 0", "argSetSize");
            return combinationsImpl(argList, 0, argSetSize - 1);
        }

        private static IEnumerable<T[]> combinationsImpl<T>(IList<T> argList, int argStart, int argIteration, List<int> argIndicies = null)
        {
            argIndicies = argIndicies ?? new List<int>();
            for (int i = argStart; i < argList.Count; i++)
            {
                argIndicies.Add(i);
                if (argIteration > 0)
                {
                    foreach (var array in combinationsImpl(argList, i + 1, argIteration - 1, argIndicies))
                    {
                        yield return array;
                    }
                }
                else
                {
                    var array = new T[argIndicies.Count];
                    for (int j = 0; j < argIndicies.Count; j++)
                    {
                        array[j] = argList[argIndicies[j]];
                    }

                    yield return array;
                }
                argIndicies.RemoveAt(argIndicies.Count - 1);
            }
        }

        public static long Factorial(long x)
        {
            if (x <= 1)
                return 1;
            else
                return x * Factorial(x - 1);
        }

        public static long Permutation(long n, long r)
        {
            if (r == 0)
                return 0;
            if (n == 0)
                return 0;
            if ((r >= 0) && (r <= n))
                return Factorial(n) / Factorial(n - r);
            else
                return 0;
        }

        public static long Combination(long a, long b)
        {
            if (a <= 1)
                return 1;

            return Factorial(a) / (Factorial(b) * Factorial(a - b));
        }

        public static double NormalizeData(double value, double dataMin, double dataMax, double minBound, double maxBound)
        {
            double range = dataMax - dataMin;

            var d1 = (value - dataMin) / range;
            return (double)((1 - d1) * minBound + d1 * maxBound);
        }

    }
}
