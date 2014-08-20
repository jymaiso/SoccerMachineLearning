using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;


namespace TestSoc
{
    public class Program
    {
        static void Main(string[] args)
        {
            Model cache = new Model();
            cache.LoadAndProcessData(new Parameters
             {
                 Function = new ExpFunction(),
                 GameCount = 74
             });

            QuotesModel qs = new QuotesModel(cache);
            qs.LoadData();

            //RunSimpleExport(cache);
            //RunSimplePrediction(cache);

            double pot = 100;
            int bet = 0;
            foreach (var quote in qs.Quotes)
            {
                if (false)
                {
                    var s1 = quote.Team1.StatsHistory.Where(a => a.Date < quote.Date).OrderByDescending(a => a.Date).First();
                    var s2 = quote.Team2.StatsHistory.Where(a => a.Date < quote.Date).OrderByDescending(a => a.Date).First();



                    var d1 = s1.ProbHomeWin - quote.R1;
                    var d2 = s2.ProbExtWin - quote.R2;

                    if (d1 > 0.2 && d1 > d2)
                    {
                        if (quote.Game.Winner == quote.Team1)
                            pot += quote.Q1;
                        else
                            pot -= 1;

                        bet++;
                    }
                    else if (d2 > 0.2 && d2 > d1)
                    {
                        if (quote.Game.Winner == quote.Team2)
                            pot += quote.Q2;
                        else
                            pot -= 1;

                        bet++;
                    }
                }
                else
                {
                    var rank1 = quote.Team1.StatsHistory.Where(a => a.Date < quote.Date).OrderByDescending(a => a.Date).First();
                    var rank2 = quote.Team2.StatsHistory.Where(a => a.Date < quote.Date).OrderByDescending(a => a.Date).First();

                    var probTie = (rank1.ProbTie + rank2.ProbTie + rank1.ProbHomeTie + rank2.ProbExtTie) / 4;
                    var probWin1 = (rank1.ProbWin + rank1.ProbHomeWin + rank2.ProbExtLoose) / 3;
                    var probWin2 = (rank2.ProbWin + rank2.ProbExtWin + rank1.ProbHomeLoose) / 3;

                    //var probTie = (rank1.ProbHomeTie + rank2.ProbExtTie) / 2;
                    //var probWin1 = (rank1.ProbHomeWin + rank2.ProbExtLoose) / 2;
                    //var probWin2 = (rank2.ProbExtWin + rank1.ProbHomeLoose) / 2;

                    if ((probTie >= probWin1 && probTie >= probWin2) || Math.Abs(probWin1 - probWin2) < 0.1)
                    {

                        if (quote.Game.Winner == null)
                            pot += (quote.QT * pot / 10);
                        else
                            pot -= pot / 10;

                        bet++;
                    }
                    else if (probWin1 >= probWin2)
                    {
                        if (quote.Game.Winner == quote.Team1)
                            pot += (quote.Q1* pot / 10);
                        else
                            pot -= pot / 10;

                        bet++;
                    }
                    else
                    {
                        if (quote.Game.Winner == quote.Team2)
                            pot += (quote.Q2 * pot / 10);
                        else
                            pot -= pot / 10;

                        bet++;
                    }
                }
                Console.WriteLine(pot);
            }

            Console.WriteLine(qs.Quotes.Count);
            Console.WriteLine(bet);
            Console.WriteLine(pot);
            Console.WriteLine("end");
            Console.ReadKey();
        }

        public static void RunSimpleExport(Model cache)
        {
            StringBuilder sb = new StringBuilder();

            List<String> labels = new List<string>(){
                "ProbTie1",
                "ProbWin1", 
                "ProbLoose1", 
                "ProbHomeTie1",
                "ProbHomeWin1", 
                "ProbHomeLoose1",
                //"ProbExtTie1",  
                //"ProbExtWin1",  
                //"ProbExtLoose1", 
                "ProbTie2",
                "ProbWin2", 
                "ProbLoose2", 
                //"ProbHomeTie2",
                //"ProbHomeWin2", 
                //"ProbHomeLoose2",
                "ProbExtTie2",  
                "ProbExtWin2",  
                "ProbExtLoose2", 
                "game.Result",
            };

            //ProbTie
            //ProbWin 
            //ProbLoose 
            //ProbHomeTie
            //ProbHomeWin 
            //ProbHomeLoose
            //ProbExtTie  
            //ProbExtWin  
            //ProbExtLoose 

            sb.AppendLine(String.Join(",", labels));

            for (int i = 0; i < cache.Games.Count - 1000; i++)
            {
                var index = cache.Games.Count - i - 1;
                var game = cache.Games[index];

                Team team1 = game.Team1;
                Team team2 = game.Team2;

                var rank1 = team1.StatsHistory.First(a => a.Date == game.Date);
                var rank2 = team2.StatsHistory.First(a => a.Date == game.Date);

                List<String> entry = new List<String>(){
                  
                     rank1.ProbTie         .TS(),
                     rank1.ProbWin         .TS(),
                     rank1.ProbLoose       .TS(),
                     rank1.ProbHomeTie     .TS(),
                     rank1.ProbHomeWin     .TS(),
                     rank1.ProbHomeLoose   .TS(),
                     //rank1.ProbExtTie      .TS(),
                     //rank1.ProbExtWin      .TS(),
                     //rank1.ProbExtLoose    .TS(),
                                
                     rank2.ProbTie         .TS(),
                     rank2.ProbWin         .TS(),
                     rank2.ProbLoose       .TS(),
                     //rank2.ProbHomeTie     .TS(),
                     //rank2.ProbHomeWin     .TS(),
                     //rank2.ProbHomeLoose   .TS(),
                     rank2.ProbExtTie      .TS(),
                     rank2.ProbExtWin      .TS(),
                     rank2.ProbExtLoose    .TS(),

                    game.Result.ToString(),
                };

                if (Double.IsNaN(rank1.ProbTie) ||
                    Double.IsNaN(rank1.ProbWin) ||
                    Double.IsNaN(rank1.ProbLoose) ||
                    Double.IsNaN(rank1.ProbHomeTie) ||
                    Double.IsNaN(rank1.ProbHomeWin) ||
                    Double.IsNaN(rank1.ProbHomeLoose) ||
                    //Double.IsNaN(rank1.ProbExtTie) ||
                    //Double.IsNaN(rank1.ProbExtWin) ||
                    //Double.IsNaN(rank1.ProbExtLoose) ||
                    Double.IsNaN(rank2.ProbTie) ||
                    Double.IsNaN(rank2.ProbWin) ||
                    Double.IsNaN(rank2.ProbLoose) ||
                    //Double.IsNaN(rank2.ProbHomeTie) ||
                    //Double.IsNaN(rank2.ProbHomeWin) ||
                    //Double.IsNaN(rank2.ProbHomeLoose) ||
                    Double.IsNaN(rank2.ProbExtTie) ||
                    Double.IsNaN(rank2.ProbExtWin) ||
                    Double.IsNaN(rank2.ProbExtLoose))
                {

                }
                else
                    sb.AppendLine(String.Join(",", entry));
            }

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "data.csv");
            File.WriteAllText(file, sb.ToString());
            Process.Start(file);
        }

        private static void RunSimplePrediction(Model cache)
        {
            List<Tuple<GameResult, GameResult>> predResults = new List<Tuple<GameResult, GameResult>>();

            for (int i = 0; i < cache.Games.Count - 1000; i++)
            {
                var index = cache.Games.Count - i - 1;
                var game = cache.Games[index];

                Team team1 = game.Team1;
                Team team2 = game.Team2;

                var rank1 = team1.StatsHistory.First(a => a.Date == game.Date);
                var rank2 = team2.StatsHistory.First(a => a.Date == game.Date);

                var probTie = (rank1.ProbTie + rank2.ProbTie + rank1.ProbHomeTie + rank2.ProbExtTie) / 4;
                var probWin1 = (rank1.ProbWin + rank1.ProbHomeWin + rank2.ProbExtLoose) / 3;
                var probWin2 = (rank2.ProbWin + rank2.ProbExtWin + rank1.ProbHomeLoose) / 3;

                //var probTie = (rank1.ProbHomeTie + rank2.ProbExtTie) / 2;
                //var probWin1 = (rank1.ProbHomeWin + rank2.ProbExtLoose) / 2;
                //var probWin2 = (rank2.ProbExtWin + rank1.ProbHomeLoose) / 2;

                if ((probTie >= probWin1 && probTie >= probWin2) || Math.Abs(probWin1 - probWin2) < 0.1)
                    predResults.Add(new Tuple<GameResult, GameResult>(GameResult.Tie, game.Result));
                else if (probWin1 >= probWin2)
                    predResults.Add(new Tuple<GameResult, GameResult>(GameResult.T1, game.Result));
                else
                    predResults.Add(new Tuple<GameResult, GameResult>(GameResult.T2, game.Result));

                //predResults.Add(new Tuple<Result, Result>(predResult, game.Result));

                //if (probWin1 >= probWin2 )
                //{
                //    predResults.Add(new Tuple<GameResult, GameResult>(GameResult.T1, game.Result));
                //}

                //if (probWin2 > probWin1 )
                //{
                //    predResults.Add(new Tuple<GameResult, GameResult>(GameResult.T2, game.Result));
                //}

                //if (probTie >= probWin1 && probTie >= probWin2)
                //    predResult = Result.Tie;
                //else if (probWin1 >= probWin2)
                //    predResult = Result.T1;
                //else
                //    predResult = Result.T2;



            }

            Console.WriteLine("Count : " + predResults.Count);
            Console.WriteLine("Score : " + Math.Round(predResults.Count(a => a.Item1 == a.Item2) / (double)predResults.Count, 2));
            Console.WriteLine("Score Tie: " + Math.Round(predResults.Where(a => a.Item1 == GameResult.Tie).Count(a => a.Item1 == a.Item2) / (double)predResults.Where(a => a.Item1 == GameResult.Tie).Count(), 2));
            Console.WriteLine("Score T1: " + Math.Round(predResults.Where(a => a.Item1 == GameResult.T1).Count(a => a.Item1 == a.Item2) / (double)predResults.Where(a => a.Item1 == GameResult.T1).Count(), 2));
            Console.WriteLine("Score T2: " + Math.Round(predResults.Where(a => a.Item1 == GameResult.T2).Count(a => a.Item1 == a.Item2) / (double)predResults.Where(a => a.Item1 == GameResult.T2).Count(), 2));

            Console.WriteLine();
            Console.WriteLine("--- Real ---");
            Console.WriteLine("Tie : " + predResults.Count(a => a.Item2 == GameResult.Tie));
            Console.WriteLine("T1 : " + predResults.Count(a => a.Item2 == GameResult.T1));
            Console.WriteLine("T2 : " + predResults.Count(a => a.Item2 == GameResult.T2));
            Console.WriteLine();
            Console.WriteLine("--- Prediction ---");
            Console.WriteLine("Tie : " + predResults.Count(a => a.Item1 == GameResult.Tie));
            Console.WriteLine("T1 : " + predResults.Count(a => a.Item1 == GameResult.T1));
            Console.WriteLine("T2 : " + predResults.Count(a => a.Item1 == GameResult.T2));



        }
    }

}
