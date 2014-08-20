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
            Cache cache = new Cache();
            cache.Go(new Parameters
             {
                 Function = new ExpFunction(),
                 GameCount = 74
             });
            //RunSimpleExport(cache);
            RunSimplePrediction(cache);

            Console.ReadKey();
        }

        public static void RunSimpleExport(Cache cache)
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

        private static void RunSimplePrediction(Cache cache)
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



    public class Cache
    {
        public List<Team> Teams { get; set; }
        public List<Game> Games { get; set; }

        public Cache()
        {
            Teams = new List<Team>();
            Games = new List<Game>();
        }



        internal void AddGames(string file, int year)
        {
            var excel = new ExcelQueryFactory(file);
            var games = from c in excel.Worksheet<ExcelGame>(String.Format("{0}_{1}", year, year + 1))
                        select c;


            foreach (var game in games)
            {
                AddGame(game, year);
            }


        }

        internal void AddGame(ExcelGame exGame, int year)
        {
            Team team1 = Teams.Where(a => a.Name.ToLower() == exGame.Team1.ToLower().Trim()).FirstOrDefault();
            if (team1 == null)
            {
                team1 = new Team { Name = exGame.Team1.Trim() };
                Teams.Add(team1);
            }
            team1.PlayThisYear(year);

            Team team2 = Teams.Where(a => a.Name.ToLower() == exGame.Team2.ToLower().Trim()).FirstOrDefault();
            if (team2 == null)
            {
                team2 = new Team { Name = exGame.Team2.Trim() };
                Teams.Add(team2);
            }
            team2.PlayThisYear(year);

            var arr = exGame.Date.Split('.');
            var dayAndMonth = new { day = Convert.ToInt32(arr[0]), month = Convert.ToInt32(arr[1]) };

            DateTime date = (dayAndMonth.month > 6) ? new DateTime(year, dayAndMonth.month, dayAndMonth.day) : new DateTime(year + 1, dayAndMonth.month, dayAndMonth.day);

            int indexOfSeparator = exGame.Score.IndexOf(":");
            var Score1 = Convert.ToInt32(exGame.Score.Substring(0, indexOfSeparator).Trim());
            var Score2 = Convert.ToInt32(exGame.Score.Substring(indexOfSeparator + 1, exGame.Score.Length - indexOfSeparator - 1).Trim());

            Team winner = null;
            if (Score1 > Score2)
                winner = team1;
            else if (Score1 < Score2)
                winner = team2;

            Game newGame = new Game
            {
                Date = date,
                Team1 = team1,
                Team2 = team2,
                Score1 = Score1,
                Score2 = Score2,
                Winner = winner
            };

            this.Games.Add(newGame);
            team1.Games.Add(newGame);
            team2.Games.Add(newGame);
        }
        public void Go(Parameters Parameters)
        {
            string file = Path.Combine(Environment.CurrentDirectory, "data.xlsx");

            this.AddGames(file, 1998);
            this.AddGames(file, 1999);
            this.AddGames(file, 2000);
            this.AddGames(file, 2001);
            this.AddGames(file, 2002);
            this.AddGames(file, 2003);
            this.AddGames(file, 2004);
            this.AddGames(file, 2005);
            this.AddGames(file, 2006);
            this.AddGames(file, 2007);
            this.AddGames(file, 2008);
            this.AddGames(file, 2009);
            this.AddGames(file, 2010);
            this.AddGames(file, 2011);
            this.AddGames(file, 2012);
            this.AddGames(file, 2013);
            this.Process(Parameters);
        }

        private void Process(Parameters Parameters)
        {

            //Re-order
            Games = Games.OrderBy(a => a.Date).ToList();

            var dates = Games.Select(a => a.Date).Distinct().OrderBy(a => a).ToList();

            Dictionary<Team, Dictionary<DateTime, TeamResult>> scoreTeamsHistory = new Dictionary<Team, Dictionary<DateTime, TeamResult>>();
            Teams.ForEach(a => scoreTeamsHistory.Add(a, new Dictionary<DateTime, TeamResult>()));

            foreach (var team in Teams)
            {
                foreach (var date in dates)
                {
                    TeamResult score = TeamResult.None;
                    var game = team.Games.FirstOrDefault(a => a.Date == date);
                    if (game != null)
                    {
                        if (game.Winner == team)
                            score = TeamResult.Win;
                        else if (game.Winner == null)
                            score = TeamResult.Tie;
                        else if (game.Winner != null && game.Winner != team)
                            score = TeamResult.Loose;

                        scoreTeamsHistory[team].Add(date, score);
                    }
                }
            }

            int gameCount = Parameters.GameCount;

            //--------------- Points and Rank ---------------------------------- 
            foreach (var date in dates)
            {
                List<TeamStats> localRanks = new List<TeamStats>();
                foreach (var team in Teams)
                {
                    var recentGames = scoreTeamsHistory[team]
                        .Where(a => a.Key < date)
                        .OrderByDescending(a => a.Key)
                        .Take(gameCount)
                        .ToList();

                    for (int i = 0; i < recentGames.Count - 1; i++)
                    {
                        if ((recentGames[i].Key - recentGames[i + 1].Key).TotalDays > 365)
                        {
                            recentGames = recentGames.Take(i + 1).ToList();
                            break;
                        }
                    }


                    var points = recentGames.Where(a => team.HavePlayThatDateTime(a.Key))
                                            .Sum(a => (a.Value == TeamResult.Win) ? 3 : ((a.Value == TeamResult.Loose) ? 0 : 1));

                    TeamStats stat = new TeamStats
                    {
                        GameCount = gameCount,
                        Date = date,
                        Team = team,
                        Points = points,
                    };
                    team.StatsHistory.Add(stat);
                    localRanks.Add(stat);

                    var game = team.Games.FirstOrDefault(a => a.Date == date);
                    if (game != null)
                    {
                        if (game.Team1 == team)
                            game.Stat1 = stat;
                        else if (game.Team2 == team)
                            game.Stat2 = stat;

                        stat.Game = game;
                    }
                }

                //int pos = 1;
                //foreach (var item in localRanks.OrderByDescending(a => a.Points).ToList())
                //{
                //    if (pos <= 20)
                //        item.Rank = pos++;
                //    else
                //        item.Rank = 20;
                //}
            }

            double dataMin = Teams.SelectMany(a => a.StatsHistory).Min(a => a.Points);
            double dataMax = Teams.SelectMany(a => a.StatsHistory).Max(a => a.Points);
            Teams.SelectMany(a => a.StatsHistory).ToList().ForEach(a => a.Points = NormalizeData(a.Points, dataMin, dataMax, -1, 1));

            //dataMin = Teams.SelectMany(a => a.RankHistory).Min(a => a.Rank);
            //dataMax = Teams.SelectMany(a => a.RankHistory).Max(a => a.Rank);
            //Teams.SelectMany(a => a.RankHistory).ToList().ForEach(a => a.Rank = NormalizeData(a.Rank, dataMin, dataMax, -1, 1));

            //--------------- Weighted prob ---------------------------------- 
            foreach (var date in dates)
            {
                foreach (var team in Teams)
                {
                    var rank = team.StatsHistory.Where(a => a.Date == date).First();

                    var games = team.Games.Where(a => a.Date < date).OrderByDescending(a => a.Date).Take(gameCount).OrderBy(a => a.Date).ToList();
                    var homeGames = games.Where(a => a.Team1 == team).OrderBy(a => a.Date).ToList();
                    var extGames = games.Where(a => a.Team1 != team).OrderBy(a => a.Date).ToList();

                    if (games.Count > 0)
                    {
                        double coeff = 0;
                        for (int i = 0; i < games.Count; i++)
                        {
                            var game = games[i];
                            var stat = game.GetStat(team);
                            var statOposite = game.GetOpositeStat(team);

                            var levelDiff = 1 + statOposite.Points;// / 2;

                            if (game.Winner == null)
                                rank.ProbTie += Parameters.Function.Y(i);

                            if (game.Winner == team)
                                rank.ProbWin += Parameters.Function.Y(i) * (Parameters.EnableStrongWeakOpposite ? levelDiff : 1);

                            if (game.Winner != null && game.Winner != team)
                                rank.ProbLoose += Parameters.Function.Y(i) * (Parameters.EnableStrongWeakOpposite ? (2 - levelDiff) : 1);

                            coeff += Parameters.Function.Y(i);
                        }

                        rank.ProbTie = coeff == 0 ? 0 : rank.ProbTie / coeff;
                        rank.ProbWin = coeff == 0 ? 0 : rank.ProbWin / coeff;
                        rank.ProbLoose = coeff == 0 ? 0 : rank.ProbLoose / coeff;
                    }

                    if (homeGames.Count > 0)
                    {
                        double coeff = 0;
                        for (int i = 0; i < homeGames.Count; i++)
                        {
                            var game = homeGames[i];
                            var stat = game.GetStat(team);
                            var statOposite = game.GetOpositeStat(team);

                            var levelDiff = 1 + statOposite.Points;// / 2;

                            if (game.Winner == null)
                                rank.ProbHomeTie += Parameters.Function.Y(i);

                            if (game.Winner == team)
                                rank.ProbHomeWin += Parameters.Function.Y(i) * (Parameters.EnableStrongWeakOpposite ? levelDiff : 1);

                            if (game.Winner != null && game.Winner != team)
                                rank.ProbHomeLoose += Parameters.Function.Y(i) * (Parameters.EnableStrongWeakOpposite ? (2 - levelDiff) : 1);

                            coeff += Parameters.Function.Y(i);
                        }

                        rank.ProbHomeTie = coeff == 0 ? 0 : rank.ProbHomeTie / coeff;
                        rank.ProbHomeWin = coeff == 0 ? 0 : rank.ProbHomeWin / coeff;
                        rank.ProbHomeLoose = coeff == 0 ? 0 : rank.ProbHomeLoose / coeff;


                    }

                    if (extGames.Count > 0)
                    {
                        double coeff = 0;
                        for (int i = 0; i < extGames.Count; i++)
                        {
                            var game = extGames[i];
                            var stat = game.GetStat(team);
                            var statOposite = game.GetOpositeStat(team);

                            var levelDiff = 1 + statOposite.Points;// / 2;
                            if (game.Winner == null)
                                rank.ProbExtTie += Parameters.Function.Y(i);

                            if (game.Winner == team)
                                rank.ProbExtWin += Parameters.Function.Y(i) * (Parameters.EnableStrongWeakOpposite ? levelDiff : 1);

                            if (game.Winner != null && game.Winner != team)
                                rank.ProbExtLoose += Parameters.Function.Y(i) * (Parameters.EnableStrongWeakOpposite ? (2 - levelDiff) : 1);

                            coeff += Parameters.Function.Y(i);
                        }

                        rank.ProbExtTie = coeff == 0 ? 0 : rank.ProbExtTie / coeff;
                        rank.ProbExtWin = coeff == 0 ? 0 : rank.ProbExtWin / coeff;
                        rank.ProbExtLoose = coeff == 0 ? 0 : rank.ProbExtLoose / coeff;

                        if (double.IsNaN(rank.ProbHomeTie) || double.IsNaN(rank.ProbHomeWin) || double.IsNaN(rank.ProbHomeLoose))
                        {
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        private static double NormalizeData(double value, double dataMin, double dataMax, double minBound, double maxBound)
        {
            double range = dataMax - dataMin;

            var d1 = (value - dataMin) / range;
            return (double)((1 - d1) * minBound + d1 * maxBound);
        }





    }

}
