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
            Cache.Instance.Go();
            //RunSimpleExport();
            RunSimplePrediction();

            Console.ReadKey();
        }

        public static void RunSimpleExport()
        {
            StringBuilder sb = new StringBuilder();

            List<String> labels = new List<string>(){
                "ProbTie1",
                "ProbWin1", 
                "ProbLoose1", 
                "ProbHomeTie1",
                "ProbHomeWin1", 
                "ProbHomeLoose1",
                "ProbExtTie1",  
                "ProbExtWin1",  
                "ProbExtLoose1", 
                "ProbTie2",
                "ProbWin2", 
                "ProbLoose2", 
                "ProbHomeTie2",
                "ProbHomeWin2", 
                "ProbHomeLoose2",
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

            for (int i = 0; i < Cache.Instance.Games.Count - 1000; i++)
            {
                var index = Cache.Instance.Games.Count - i - 1;
                var game = Cache.Instance.Games[index];

                Team team1 = game.Team1;
                Team team2 = game.Team2;

                var rank1 = team1.RankHistory.First(a => a.Date == game.Date);
                var rank2 = team2.RankHistory.First(a => a.Date == game.Date);

                List<String> entry = new List<String>(){
                  
                     rank1.ProbTie         .TS(),
                     rank1.ProbWin         .TS(),
                     rank1.ProbLoose       .TS(),
                     rank1.ProbHomeTie     .TS(),
                     rank1.ProbHomeWin     .TS(),
                     rank1.ProbHomeLoose   .TS(),
                     rank1.ProbExtTie      .TS(),
                     rank1.ProbExtWin      .TS(),
                     rank1.ProbExtLoose    .TS(),
                                
                     rank2.ProbTie         .TS(),
                     rank2.ProbWin         .TS(),
                     rank2.ProbLoose       .TS(),
                     rank2.ProbHomeTie     .TS(),
                     rank2.ProbHomeWin     .TS(),
                     rank2.ProbHomeLoose   .TS(),
                     rank2.ProbExtTie      .TS(),
                     rank2.ProbExtWin      .TS(),
                     rank2.ProbExtLoose    .TS(),

                    game.Result.ToString(),
                };

                sb.AppendLine(String.Join(",", entry));
            }

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "data.csv");
            File.WriteAllText(file, sb.ToString());
            Process.Start(file);
        }

        private static void RunSimplePrediction()
        {
            List<Tuple<Result, Result>> predResults = new List<Tuple<Result, Result>>();

            for (int i = 0; i < Cache.Instance.Games.Count - 1000; i++)
            {
                var index = Cache.Instance.Games.Count - i - 1;
                var game = Cache.Instance.Games[index];

                Team team1 = game.Team1;
                Team team2 = game.Team2;

                var rank1 = team1.RankHistory.First(a => a.Date == game.Date);
                var rank2 = team2.RankHistory.First(a => a.Date == game.Date);

                Result predResult;

                var probTie = (rank1.ProbTie + rank2.ProbTie + rank1.ProbHomeTie + rank2.ProbExtTie) / 4;
                var probWin1 = (rank1.ProbWin + rank1.ProbHomeWin + rank2.ProbExtLoose) / 3;
                var probWin2 = (rank2.ProbWin + rank2.ProbExtWin + rank1.ProbHomeLoose) / 3;

                //var probTie = (rank1.ProbHomeTie + rank2.ProbExtTie) / 2;
                //var probWin1 = (rank1.ProbHomeWin + rank2.ProbExtLoose) / 2;
                //var probWin2 = (rank2.ProbExtWin + rank1.ProbHomeLoose) / 2;

                //if (probTie >= probWin1 && probTie >= probWin2)
                //    predResult = Result.Tie;
                //else if (probWin1 >= probWin2)
                //    predResult = Result.T1;
                //else
                //    predResult = Result.T2;

                //predResults.Add(new Tuple<Result, Result>(predResult, game.Result));

                if (probWin1 > probWin2 * 2)
                {
                    predResults.Add(new Tuple<Result, Result>(Result.T1, game.Result));
                }

                if (probWin2 > probWin1 * 2)
                {
                    predResults.Add(new Tuple<Result, Result>(Result.T2, game.Result));
                }

                //if (probTie >= probWin1 && probTie >= probWin2)
                //    predResult = Result.Tie;
                //else if (probWin1 >= probWin2)
                //    predResult = Result.T1;
                //else
                //    predResult = Result.T2;

              

            }

            Console.WriteLine("Count : " + predResults.Count);
            Console.WriteLine("Score : " + Math.Round(predResults.Count(a => a.Item1 == a.Item2) / (double)predResults.Count, 2));
            Console.WriteLine("Score Tie: " + Math.Round(predResults.Where(a => a.Item1 == Result.Tie).Count(a => a.Item1 == a.Item2) / (double)predResults.Where(a => a.Item1 == Result.Tie).Count(), 2));
            Console.WriteLine("Score T1: " + Math.Round(predResults.Where(a => a.Item1 == Result.T1).Count(a => a.Item1 == a.Item2) / (double)predResults.Where(a => a.Item1 == Result.T1).Count(), 2));
            Console.WriteLine("Score T2: " + Math.Round(predResults.Where(a => a.Item1 == Result.T2).Count(a => a.Item1 == a.Item2) / (double)predResults.Where(a => a.Item1 == Result.T2).Count(), 2));

            Console.WriteLine();
            Console.WriteLine("--- Real ---");
            Console.WriteLine("Tie : " + predResults.Count(a => a.Item2 == Result.Tie));
            Console.WriteLine("T1 : " + predResults.Count(a => a.Item2 == Result.T1));
            Console.WriteLine("T2 : " + predResults.Count(a => a.Item2 == Result.T2));
            Console.WriteLine();
            Console.WriteLine("--- Prediction ---");
            Console.WriteLine("Tie : " + predResults.Count(a => a.Item1 == Result.Tie));
            Console.WriteLine("T1 : " + predResults.Count(a => a.Item1 == Result.T1));
            Console.WriteLine("T2 : " + predResults.Count(a => a.Item1 == Result.T2));



        }
    }

    public static class Ext
    {
        public static String TS(this double num)
        {
            return num.ToString().Replace(",", ".");
        }
    }

    public enum Result
    {
        T1, T2, Tie

    }

    public class ExcelGame
    {
        public String Date { get; set; }
        public String Team1 { get; set; }
        public String Team2 { get; set; }
        public String Score { get; set; }

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

        private static readonly Cache instance = new Cache();

        public static Cache Instance
        {
            get
            {
                return instance;
            }
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

        internal void AddGame(ExcelGame exGame, int year, bool addGameToTeam = true)
        {
            Team team1 = Teams.Where(a => a.Name.ToLower() == exGame.Team1.ToLower().Trim()).FirstOrDefault();
            if (team1 == null)
            {
                team1 = new Team { Name = exGame.Team1.Trim() };
                Teams.Add(team1);
            }

            Team team2 = Teams.Where(a => a.Name.ToLower() == exGame.Team2.ToLower().Trim()).FirstOrDefault();
            if (team2 == null)
            {
                team2 = new Team { Name = exGame.Team2.Trim() };
                Teams.Add(team2);
            }

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

            if (addGameToTeam)
            {
                team1.Games.Add(newGame);
                team2.Games.Add(newGame);
            }

        }

        public void Go()
        {
            string file = Path.Combine(Environment.CurrentDirectory, "data.xlsx");


            Cache.Instance.AddGames(file, 1998);
            Cache.Instance.AddGames(file, 1999);
            Cache.Instance.AddGames(file, 2000);
            Cache.Instance.AddGames(file, 2001);
            Cache.Instance.AddGames(file, 2002);
            Cache.Instance.AddGames(file, 2003);
            Cache.Instance.AddGames(file, 2004);
            Cache.Instance.AddGames(file, 2005);
            Cache.Instance.AddGames(file, 2006);
            Cache.Instance.AddGames(file, 2007);
            Cache.Instance.AddGames(file, 2008);
            Cache.Instance.AddGames(file, 2009);
            Cache.Instance.AddGames(file, 2010);
            Cache.Instance.AddGames(file, 2011);
            Cache.Instance.AddGames(file, 2012);
            Cache.Instance.AddGames(file, 2013);
            Cache.Instance.Process();
        }

        internal void Process()
        {
            //Re-order
            Games = Games.OrderBy(a => a.Date).ToList();

            var dates = Games.Select(a => a.Date).Distinct().OrderBy(a => a).ToList();

            Dictionary<Team, Dictionary<DateTime, int>> scoreTeamsHistory = new Dictionary<Team, Dictionary<DateTime, int>>();
            Teams.ForEach(a => scoreTeamsHistory.Add(a, new Dictionary<DateTime, int>()));

            foreach (var team in Teams)
            {
                foreach (var date in dates)
                {
                    int score = 0;
                    var game = team.Games.FirstOrDefault(a => a.Date == date);
                    if (game != null)
                        if (game.Winner == team)
                            score = 1;
                        else if (game.Winner != null)
                            score = -1;

                    scoreTeamsHistory[team].Add(date, score);
                }
            }

            int gameCount = 38;
            foreach (var date in dates)
            {
                List<TeamStats> localRanks = new List<TeamStats>();
                foreach (var team in Teams)
                {
                    var points = scoreTeamsHistory[team].Where(a => a.Key < date).OrderByDescending(a => a.Key).Take(gameCount).Sum(a => a.Value);
                    var games = team.Games.Where(a => a.Date < date).OrderByDescending(a => a.Date).Take(gameCount).ToList();
                    var homeGames = games.Where(a => a.Team1 == team).ToList();
                    var extGames = games.Where(a => a.Team1 != team).ToList();

                    TeamStats rank = new TeamStats
                    {
                        GameCount = gameCount,
                        Date = date,
                        Team = team,
                        Points = points,
                        ProbTie = games.Count != 0 ? games.Count(a => a.Winner == null) / (double)games.Count : 0,
                        ProbWin = games.Count != 0 ? games.Count(a => a.Winner == team) / (double)games.Count : 0,
                        ProbLoose = games.Count != 0 ? games.Count(a => a.Winner != null && a.Winner != team) / (double)games.Count : 0,
                        ProbHomeTie = homeGames.Count != 0 ? homeGames.Count(a => a.Winner == null) / (double)homeGames.Count : 0,
                        ProbHomeWin = homeGames.Count != 0 ? homeGames.Count(a => a.Winner == team) / (double)homeGames.Count : 0,
                        ProbHomeLoose = homeGames.Count != 0 ? homeGames.Count(a => a.Winner != null && a.Winner != team) / (double)homeGames.Count : 0,
                        ProbExtTie = extGames.Count != 0 ? extGames.Count(a => a.Winner == null) / (double)extGames.Count : 0,
                        ProbExtWin = extGames.Count != 0 ? extGames.Count(a => a.Winner == team) / (double)extGames.Count : 0,
                        ProbExtLoose = extGames.Count != 0 ? extGames.Count(a => a.Winner != null && a.Winner != team) / (double)extGames.Count : 0
                    };
                    team.RankHistory.Add(rank);
                    localRanks.Add(rank);
                }

                int pos = 1;
                foreach (var item in localRanks.OrderByDescending(a => a.Points).ToList())
                {
                    item.Rank = pos++;
                }
            }


            double dataMin = Teams.SelectMany(a => a.RankHistory).Min(a => a.Points);
            double dataMax = Teams.SelectMany(a => a.RankHistory).Max(a => a.Points);
            Teams.SelectMany(a => a.RankHistory).ToList().ForEach(a => a.Points = NormalizeData(a.Points, dataMin, dataMax, -1, 1));

            dataMin = Teams.SelectMany(a => a.RankHistory).Min(a => a.Rank);
            dataMax = Teams.SelectMany(a => a.RankHistory).Max(a => a.Rank);
            Teams.SelectMany(a => a.RankHistory).ToList().ForEach(a => a.Rank = NormalizeData(a.Rank, dataMin, dataMax, -1, 1));
        }

        private static double NormalizeData(double value, double dataMin, double dataMax, double minBound, double maxBound)
        {
            double range = dataMax - dataMin;

            var d1 = (value - dataMin) / range;
            return (double)((1 - d1) * minBound + d1 * maxBound);
        }

    }

    public class Team
    {
        public String Name { get; set; }
        public List<Game> Games { get; set; }

        public List<TeamStats> RankHistory { get; set; }


        public Team()
        {
            Games = new List<Game>();
            RankHistory = new List<TeamStats>();
        }
    }

    public class Game
    {
        public DateTime Date { get; set; }

        public Team Team1 { get; set; }
        public Team Team2 { get; set; }

        public int Score1 { get; set; }
        public int Score2 { get; set; }

        public Team Winner { get; set; }

        private Result myVar;

        public Result Result
        {
            get
            {

                if (Winner == Team1)
                    return TestSoc.Result.T1;
                else if (Winner == Team2)
                    return TestSoc.Result.T2;

                return TestSoc.Result.Tie;
            }
        }

    }



    public class TeamStats
    {
        public int GameCount { get; set; }
        public double Rank { get; set; }
        public Team Team { get; set; }
        public DateTime Date { get; set; }

        public double Points { get; set; }

        public double ProbTie { get; set; }

        public double ProbWin { get; set; }

        public double ProbLoose { get; set; }

        public double ProbHomeTie { get; set; }

        public double ProbHomeWin { get; set; }

        public double ProbHomeLoose { get; set; }

        public double ProbExtTie { get; set; }

        public double ProbExtWin { get; set; }

        public double ProbExtLoose { get; set; }
    }

}
