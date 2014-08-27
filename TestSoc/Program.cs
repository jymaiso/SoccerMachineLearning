using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;


namespace TestSoc
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("ExpFunction");
            //{
            //    Model cache = new Model();
            //    cache.LoadAndProcessData(new Parameters
            //     {
            //         Function = new ExpFunction(3),
            //         GameCount = 74,
            //         EnableStrongWeakOpposite = false,

            //     });


            //QuotesModel qs = new QuotesModel(cache);
            //qs.LoadData();
            //ProcessQuotes(qs);
            //}

            Console.WriteLine("EnableStrongWeakOpposite");
            {
                Model cache = new Model();
                cache.LoadAndProcessData(new Parameters
                {
                    Function = new ExpFunction(3),
                    GameCount = 148,
                    EnableStrongWeakOpposite = true,

                });

                //GetCSV(cache);

                QuotesModel qs = new QuotesModel(cache);
                qs.LoadData();
                ProcessQuotes(qs);
            }

            Console.WriteLine("end");
            Console.ReadKey();
        }

        private static void GetCSV(Model cache)
        {
            List<Game> Games = cache.Games;

            Type type = typeof(TeamStats);
            List<PropertyInfo> properties = type.GetProperties().Where(a => a.CanRead && a.PropertyType == typeof(double)).ToList();
            var sb = new StringBuilder();

            // First line contains field names

            sb.Append(String.Join(",", properties.Select(a => a.Name + "_1").Union(properties.Select(a => a.Name + "_2"))));
            sb.Append(",Output");
            sb.AppendLine();

            foreach (Game game in Games)
            {

                foreach (PropertyInfo prp in properties)
                {
                    if (prp.CanRead)
                    {
                        sb.Append(prp.GetValue(game.Stat1, null).ToString().Replace(",", ".")).Append(',');
                    }
                }

                foreach (PropertyInfo prp in properties)
                {
                    if (prp.CanRead)
                    {
                        sb.Append(prp.GetValue(game.Stat2, null).ToString().Replace(",", ".")).Append(',');
                    }
                }

                sb.Append(game.Result);
                sb.AppendLine();
            }

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "data.csv");
            File.WriteAllText(file, sb.ToString());
            Process.Start(file);
        }


        private static void ProcessQuotes(QuotesModel qs)
        {
            BetPot BetPot = new BetPot();

            //ProcessBetPotKellyCriterion(qs, BetPot);

            int interval = qs.Quotes.Count / 20;

            Console.WriteLine("me");
            for (int i = 0; i < 20; i++)
            {
                var quotes = qs.Quotes.Skip(i * interval).Take(interval).ToList();
                Console.WriteLine(i + " : " + Math.Sqrt(quotes.Sum(a => a.MyMSE) / quotes.Count));
            }

            Console.WriteLine("bookie");
            for (int i = 0; i < 20; i++)
            {
                var quotes = qs.Quotes.Skip(i * interval).Take(interval).ToList();
                Console.WriteLine(i + " : " + Math.Sqrt(quotes.Sum(a => a.BookieMSE) / quotes.Count));
            }

            Console.WriteLine("RandomMSE");
            for (int i = 0; i < 20; i++)
            {
                var quotes = qs.Quotes.Skip(i * interval).Take(interval).ToList();
                Console.WriteLine(i + " : " + Math.Sqrt(quotes.Sum(a => a.RandomMSE) / quotes.Count));
            }

            //var MyMSE = Math.Sqrt(qs.Quotes.Sum(a => a.MyMSE) / qs.Quotes.Count);
            //var BookieMSE = Math.Sqrt(qs.Quotes.Sum(a => a.BookieMSE) / qs.Quotes.Count);
            //var RandomMSE = Math.Sqrt(qs.Quotes.Sum(a => a.RandomMSE) / qs.Quotes.Count);

            //Console.WriteLine("--- Mean Squared Error ---");
            //Console.WriteLine("MyMSE : " + MyMSE);
            //Console.WriteLine("BookieMSE : " + BookieMSE);
            //Console.WriteLine("RandomMSE : " + RandomMSE);

            //Console.WriteLine(qs.Quotes.Count);
            //Console.WriteLine(BetPot.bet);
            //Console.WriteLine(String.Format("True: {0}, False: {1}", BetPot.ttrue, BetPot.ffalse));
            //Console.WriteLine(BetPot.pot);
        }

        private static void ProcessBetPotKellyCriterion(QuotesModel qs, BetPot BetPot)
        {
            foreach (var quote in qs.Quotes)
            {
                var F1 = (quote.MyProb1 * quote.Q1 - 1) / (quote.Q1 - 1);
                var FT = (quote.MyProbT * quote.QT - 1) / (quote.QT - 1);
                var F2 = (quote.MyProb2 * quote.Q2 - 1) / (quote.Q2 - 1);

                if (F1 > FT && F1 > F2)
                {
                    var betValue = BetPot.pot * F1;
                    if (quote.Game.Winner == quote.Team1)
                    {
                        BetPot.ttrue++;
                        BetPot.pot += ((quote.Q1 - 1) * betValue);
                    }
                    else
                    {
                        BetPot.ffalse++;
                        BetPot.pot -= betValue;
                    }
                    BetPot.bet++;
                }
                else if (FT > F1 && FT > F2)
                {
                    var betValue = BetPot.pot * FT;
                    if (quote.Game.Winner == null)
                    {
                        BetPot.ttrue++;
                        BetPot.pot += ((quote.QT - 1) * betValue);
                    }
                    else
                    {
                        BetPot.ffalse++;
                        BetPot.pot -= betValue;
                    }
                    BetPot.bet++;
                }
                else if (F2 > FT && F2 > F1)
                {
                    var betValue = BetPot.pot * F2;
                    if (quote.Game.Winner == quote.Team2)
                    {
                        BetPot.ttrue++;
                        BetPot.pot += ((quote.Q2 - 1) * betValue);
                    }
                    else
                    {
                        BetPot.ffalse++;
                        BetPot.pot -= betValue;
                    }
                    BetPot.bet++;
                }
            }
        }
    }
}
