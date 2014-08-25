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
            double pot = 100;
            int bet = 0;
            int ttrue = 0;
            int ffalse = 0;
            StringBuilder sb = new StringBuilder();

            double myBetScore = 0;
            double bookieBetScore = 0;
            double randomBetScore = 0;

            double myRPS = 0;
            double bookieRPS = 0;
            double randomRPS = 0;

            Random rnd = new Random(DateTime.Now.Millisecond);
            foreach (var quote in qs.Quotes)
            {

                if (false)
                {

                }
                else
                {
                    int win1Coeff = quote.Game.Result == GameResult.T1 ? 1 : 0;
                    int win2Coeff = quote.Game.Result == GameResult.T2 ? 1 : 0;
                    int winTCoeff = quote.Game.Result == GameResult.Tie ? 1 : 0;

                    var s1 = quote.Team1.StatsHistory.Where(a => a.Date == quote.Date).Single();
                    var s2 = quote.Team2.StatsHistory.Where(a => a.Date == quote.Date).Single();

                    //var probTie = (s1.ProbTie + s2.ProbTie + s1.ProbHomeTie + s2.ProbExtTie) / 4;
                    //var probWin1 = (s1.ProbWin + s1.ProbHomeWin + s2.ProbExtLoose) / 3;
                    //var probWin2 = (s2.ProbWin + s2.ProbExtWin + s1.ProbHomeLoose) / 3;

                    var probTie = (s1.ProbHomeTie + s2.ProbExtTie) / 2;
                    var probWin1 = (s1.ProbHomeWin + s2.ProbExtLoose) / 2;
                    var probWin2 = (s2.ProbExtWin + s1.ProbHomeLoose) / 2;

                    var total = probTie + probWin1 + probWin2;
                    var probTieNorm = probTie / total;
                    var probWin1Norm = probWin1 / total;
                    var probWin2Norm = probWin2 / total;

                    myBetScore += probTieNorm * winTCoeff + probWin1Norm * win1Coeff + probWin2Norm * win2Coeff;
                    myRPS += 0.5 * (Math.Pow(probTieNorm - winTCoeff, 2) + Math.Pow(probWin1Norm - win1Coeff, 2) + Math.Pow(probWin2Norm - win2Coeff, 2));

                    var totalBookie = quote.R1 + quote.R2 + quote.RT;
                    var probTieNormBookie = quote.RT / totalBookie;
                    var probWin1NormBookie = quote.R1 / totalBookie;
                    var probWin2NormBookie = quote.R2 / totalBookie;


                    bookieBetScore += probTieNormBookie * winTCoeff + probWin1NormBookie * win1Coeff + probWin2NormBookie * win2Coeff;
                    bookieRPS += 0.5 * (Math.Pow(probTieNormBookie - winTCoeff, 2) + Math.Pow(probWin1NormBookie - win1Coeff, 2) + Math.Pow(probWin2NormBookie - win2Coeff, 2));

                    var betValue = pot / 10;

                    if ((probTie >= probWin1 + 0.2 && probTie >= probWin2 + 0.2))
                    {
                        if (quote.Game.Winner == null)
                        {
                            ttrue++;
                            pot += ((quote.QT - 1) * betValue);
                        }
                        else
                        {
                            ffalse++;
                            pot -= betValue;
                        }

                        bet++;
                    }
                    else
                        if (probWin1 >= probWin2 + 0.2)
                        {
                            if (quote.Game.Winner == quote.Team1)
                            {
                                ttrue++;
                                pot += ((quote.Q1 - 1) * betValue);
                            }
                            else
                            {
                                ffalse++;
                                pot -= betValue;
                            }

                            bet++;
                        }
                        else
                            if (probWin2 >= probWin1 + 0.2)
                            {
                                if (quote.Game.Winner == quote.Team2)
                                {
                                    ttrue++;
                                    pot += ((quote.Q2 - 1) * betValue);
                                }
                                else
                                {
                                    ffalse++;
                                    pot -= betValue;
                                }

                                bet++;
                            }

                    var random = rnd.Next(1, 100);
                    if ((1 <= random && random < 33 && quote.Game.Winner == quote.Team1)
                        || (33 <= random && random < 66 && quote.Game.Result == GameResult.Tie)
                        || (66 <= random && random < 100 && quote.Game.Winner == quote.Team2))
                    {
                        randomBetScore += 1;
                        Console.WriteLine();
                    }

                    randomRPS += 0.5 * (Math.Pow(0.33 - winTCoeff, 2) + Math.Pow(0.33 - win1Coeff, 2) + Math.Pow(0.33 - win2Coeff, 2));
                }
            }

            myBetScore = myBetScore / qs.Quotes.Count;
            bookieBetScore = bookieBetScore / qs.Quotes.Count;
            randomBetScore = randomBetScore / qs.Quotes.Count;

            myRPS = myRPS / qs.Quotes.Count;
            bookieRPS = bookieRPS / qs.Quotes.Count;
            randomRPS = randomRPS / qs.Quotes.Count;

            Console.WriteLine("myBetScore : " + myBetScore);
            Console.WriteLine("bookieBetScore : " + bookieBetScore);
            Console.WriteLine("randomBetScore : " + randomBetScore);

            Console.WriteLine("myRPS : " + myRPS);
            Console.WriteLine("bookieRPS : " + bookieRPS);
            Console.WriteLine("randomRPS : " + randomRPS);

            Console.WriteLine(qs.Quotes.Count);
            Console.WriteLine(bet);
            Console.WriteLine(String.Format("True: {0}, False: {1}", ttrue, ffalse));
            Console.WriteLine(pot);
        }
    }
}
