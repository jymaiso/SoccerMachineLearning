using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;
using TestSoc.DataAccess;


namespace TestSoc
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ExpFunction");
            {
                ModelGames model = new ModelGames();
                ModelQuotes qs = new ModelQuotes(model);


                for (int i = 0; i < 1; i++)
                {
                    model.ProcessData(new Parameters
                    {
                        Function = new LinearFunction(2),
                        GameCount = 38,
                        x0 = 0,
                        x1 = 226,
                        x2 = 9,
                        x3 = 1.3,
                        x4 = -0.03,
                        k = 0.2
                    });
                    qs.LoadData();
                    OutputQuotes(qs);

                    Console.WriteLine(i + " : " + Math.Sqrt(qs.Quotes.Sum(a => a.MyMSE) / qs.Quotes.Count));
                }
            }

            Console.WriteLine("end");
            Console.ReadKey();
        }

        private static void OutputQuotes(ModelQuotes qs)
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

            Console.WriteLine("HomeWinMSE : " + Math.Sqrt(qs.Quotes.Sum(a => a.HomeWinMSE) / qs.Quotes.Count));
            for (int i = 0; i < 10; i++)
            {
                var quotes = qs.Quotes.Skip(i * interval).Take(interval).ToList();
                Console.WriteLine(i + " : " + Math.Sqrt(quotes.Sum(a => a.HomeWinMSE) / quotes.Count));
            }

            Console.WriteLine("--- Correctly Classified ---");
            Console.WriteLine("MyCorrect : " + qs.Quotes.Sum(a => a.MyCorrect) / qs.Quotes.Count);
            Console.WriteLine("BookieCorrect : " + qs.Quotes.Sum(a => a.BookieCorrect) / qs.Quotes.Count);
            Console.WriteLine("HomeWin : " + qs.Quotes.Where(a => a.Game.FTR == GameResult.HomeWin).Count() / (double)qs.Quotes.Count);

            Console.WriteLine("--- Distribution ---");
            var GameResults = qs.Quotes.Select(a => a.Game.FTR).ToList();
            Console.WriteLine(String.Format("GameResults = 1 :{0:n2} / T : {1:n2} / 2 : {2:n2}", GameResults.Count(a => a == GameResult.HomeWin) * 100 / (double)qs.Quotes.Count,
                                                                          GameResults.Count(a => a == GameResult.Draw) * 100 / (double)qs.Quotes.Count,
                                                                          GameResults.Count(a => a == GameResult.AwayWin) * 100 / (double)qs.Quotes.Count));

            var myResults = qs.Quotes.Select(a => a.MyResult).ToList();
            Console.WriteLine(String.Format("myResults = 1 :{0:n2} / T : {1:n2} / 2 : {2:n2}", myResults.Count(a => a == GameResult.HomeWin) * 100 / (double)qs.Quotes.Count,
                                                                          myResults.Count(a => a == GameResult.Draw) * 100 / (double)qs.Quotes.Count,
                                                                          myResults.Count(a => a == GameResult.AwayWin) * 100 / (double)qs.Quotes.Count));

            var BookiResults = qs.Quotes.Select(a => a.BookiResult).ToList();
            Console.WriteLine(String.Format("BookieResults = 1 :{0:n2} / T : {1:n2} / 2 : {2:n2}", BookiResults.Count(a => a == GameResult.HomeWin) * 100 / (double)qs.Quotes.Count,
                                                                          BookiResults.Count(a => a == GameResult.Draw) * 100 / (double)qs.Quotes.Count,
                                                                          BookiResults.Count(a => a == GameResult.AwayWin) * 100 / (double)qs.Quotes.Count));

        }

        private static void ProcessBetPotKellyCriterion(ModelQuotes qs, BetPot BetPot)
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
