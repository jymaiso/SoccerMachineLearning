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

            //Console.WriteLine("EnableStrongWeakOpposite");
            //{
            //    Model cache = new Model();
            //    cache.LoadAndProcessData(new Parameters
            //    {
            //        Function = new ExpFunction(3),
            //        GameCount = 148,
            //        EnableStrongWeakOpposite = true,

            //    });

            //    //GetCSV(cache);

            //    QuotesModel qs = new QuotesModel(cache);
            //    qs.LoadData();
            //    ProcessQuotes(qs);

            //    BetPot BetPot = new BetPot();
            //    ProcessBetPotKellyCriterion(qs, BetPot);
            //}

            Model cache = new Model();
            cache.LoadData();
           

            for (int i = 1; i < 20; i++)
            {


                TestSoc.Quote.Singleton.Instance.Limit = i * 10;

                cache.LoadAndProcessData(new Parameters
                {
                    Function = new ExpFunction(3),
                    GameCount = 148,
                    EnableStrongWeakOpposite = true,

                });

                QuotesModel qs = new QuotesModel(cache);
                qs.LoadData();

                Console.WriteLine(String.Format("{0} ; {1} ; {2}", TestSoc.Quote.Singleton.Instance.Limit,
                                                                    Math.Sqrt(qs.Quotes.Sum(a => a.MyMSE) / qs.Quotes.Count),
                                                                    qs.Quotes.Sum(a => a.MyCorrect) / qs.Quotes.Count));

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

            var games = Games.Where(a => a.Date.Year >= 2000).ToList().Shuffle();
            var games1 = games.Where(a => a.Result == GameResult.T1).Take(1000).ToList().Shuffle();
            var gamesT = games.Where(a => a.Result == GameResult.Tie).Take(1000).ToList().Shuffle();
            var games2 = games.Where(a => a.Result == GameResult.T2).Take(1000).ToList().Shuffle();

            games = games1.Union(gamesT).Union(games2).ToList().Shuffle();

            foreach (Game game in games)
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


            int interval = qs.Quotes.Count / 10;

            Console.WriteLine("--- RMSE ---");

            Console.WriteLine("me : " + Math.Sqrt(qs.Quotes.Sum(a => a.MyMSE) / qs.Quotes.Count));
            for (int i = 0; i < 10; i++)
            {
                var quotes = qs.Quotes.Skip(i * interval).Take(interval).ToList();
                Console.WriteLine(i + " : " + Math.Sqrt(quotes.Sum(a => a.MyMSE) / quotes.Count));
            }

            Console.WriteLine("bookie : " + Math.Sqrt(qs.Quotes.Sum(a => a.BookieMSE) / qs.Quotes.Count));
            for (int i = 0; i < 10; i++)
            {
                var quotes = qs.Quotes.Skip(i * interval).Take(interval).ToList();
                Console.WriteLine(i + " : " + Math.Sqrt(quotes.Sum(a => a.BookieMSE) / quotes.Count));
            }

            Console.WriteLine("RandomMSE : " + Math.Sqrt(qs.Quotes.Sum(a => a.RandomMSE) / qs.Quotes.Count));
            for (int i = 0; i < 10; i++)
            {
                var quotes = qs.Quotes.Skip(i * interval).Take(interval).ToList();
                Console.WriteLine(i + " : " + Math.Sqrt(quotes.Sum(a => a.RandomMSE) / quotes.Count));
            }

            Console.WriteLine("--- Correctly Classified ---");
            Console.WriteLine("MyCorrect : " + qs.Quotes.Sum(a => a.MyCorrect) / qs.Quotes.Count);
            Console.WriteLine("BookieCorrect : " + qs.Quotes.Sum(a => a.BookieCorrect) / qs.Quotes.Count);

            Console.WriteLine("--- Distribution ---");
            var GameResults = qs.Quotes.Select(a => a.Game.Result).ToList();
            Console.WriteLine(String.Format("GameResults = 1 :{0:n2} / T : {1:n2} / 2 : {2:n2}", GameResults.Count(a => a == GameResult.T1) * 100 / (double)qs.Quotes.Count,
                                                                          GameResults.Count(a => a == GameResult.Tie) * 100 / (double)qs.Quotes.Count,
                                                                          GameResults.Count(a => a == GameResult.T2) * 100 / (double)qs.Quotes.Count));

            var myResults = qs.Quotes.Select(a => a.MyResult).ToList();
            Console.WriteLine(String.Format("myResults = 1 :{0:n2} / T : {1:n2} / 2 : {2:n2}", myResults.Count(a => a == GameResult.T1) * 100 / (double)qs.Quotes.Count,
                                                                          myResults.Count(a => a == GameResult.Tie) * 100 / (double)qs.Quotes.Count,
                                                                          myResults.Count(a => a == GameResult.T2) * 100 / (double)qs.Quotes.Count));

            var BookiResults = qs.Quotes.Select(a => a.BookiResult).ToList();
            Console.WriteLine(String.Format("BookieResults = 1 :{0:n2} / T : {1:n2} / 2 : {2:n2}", BookiResults.Count(a => a == GameResult.T1) * 100 / (double)qs.Quotes.Count,
                                                                          BookiResults.Count(a => a == GameResult.Tie) * 100 / (double)qs.Quotes.Count,
                                                                          BookiResults.Count(a => a == GameResult.T2) * 100 / (double)qs.Quotes.Count));

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
