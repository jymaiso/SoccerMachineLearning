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





            //RunSimpleExport();
            // RunSimplePrediction();



            Console.ReadKey();

        }

        public static void RunSimpleExport()
        {
            StringBuilder sb = new StringBuilder();

            List<String> labels = new List<string>(){
                 "p1",
                 "r1",
                 "p1",
                 "r2",
                    
                    "game.Result",
            };

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
                rank1.Points.ToString().Replace(",","."),
                  rank1.Rank.ToString().Replace(",","."),
                  rank2.Points.ToString().Replace(",","."),
                  rank2.Rank.ToString().Replace(",","."),
                    
                    game.Result.ToString().Replace(",","."),
            };

                sb.AppendLine(String.Join(",", entry));

            }

            string file = Path.Combine(Environment.CurrentDirectory, "data.csv");
            File.WriteAllText(file, sb.ToString());
            Process.Start(file);

        }

        private static void RunSimplePrediction()
        {
            List<Tuple<Result, Result>> predResults = new List<Tuple<Result, Result>>();

            for (int i = 0; i < 1000; i++)
            {
                var index = Cache.Instance.Games.Count - i - 1;
                var game = Cache.Instance.Games[index];

                Team team1 = game.Team1;
                Team team2 = game.Team2;

                var confront1 = Cache.Instance.Games.Where(a => a.Team1 == team1 && a.Team2 == team2).Where(a => a.Date < game.Date).ToList();
                var confront2 = Cache.Instance.Games.Where(a => a.Team1 == team2 && a.Team2 == team1).Where(a => a.Date < game.Date).ToList();

                var confronts = confront1.Union(confront2);

                var recentConfronts10 = confronts.OrderByDescending(a => a.Date).Take(10);
                double countConfront10 = recentConfronts10.Count();
                double confVictoRatioTeam1_10 = countConfront10 == 0 ? 0 : recentConfronts10.Count(a => a.Winner == team1) / countConfront10;
                double confVictoRatioTeam2_10 = countConfront10 == 0 ? 0 : recentConfronts10.Count(a => a.Winner == team2) / countConfront10;
                double confTieRatio_10 = countConfront10 == 0 ? 0 : recentConfronts10.Count(a => a.Winner == null) / countConfront10;

                var recentConfronts5 = confronts.OrderByDescending(a => a.Date).Take(5);
                double countConfront5 = recentConfronts5.Count();
                double confVictoRatioTeam1_5 = countConfront5 == 0 ? 0 : recentConfronts5.Count(a => a.Winner == team1) / countConfront5;
                double confVictoRatioTeam2_5 = countConfront5 == 0 ? 0 : recentConfronts5.Count(a => a.Winner == team2) / countConfront5;
                double confTieRatio_5 = countConfront5 == 0 ? 0 : recentConfronts5.Count(a => a.Winner == null) / countConfront5;

                var recentConfronts2 = confronts.OrderByDescending(a => a.Date).Take(2);
                double countConfront2 = recentConfronts2.Count();
                double confVictoRatioTeam1_2 = countConfront2 == 0 ? 0 : recentConfronts2.Count(a => a.Winner == team1) / countConfront2;
                double confVictoRatioTeam2_2 = countConfront2 == 0 ? 0 : recentConfronts2.Count(a => a.Winner == team2) / countConfront2;
                double confTieRatio_2 = countConfront2 == 0 ? 0 : recentConfronts2.Count(a => a.Winner == null) / countConfront2;


                var teamRecentGame1_5 = team1.Games.Where(a => a.Date < game.Date).OrderByDescending(a => a.Date).Take(5).ToList();
                double countteamRecentGame1_5 = teamRecentGame1_5.Count();
                double indVictoRatioTeam1_5 = countteamRecentGame1_5 == 0 ? 0 : teamRecentGame1_5.Count(a => a.Winner == team1) / countteamRecentGame1_5;
                double indLooseRatioTeam1_5 = countteamRecentGame1_5 == 0 ? 0 : teamRecentGame1_5.Count(a => a.Winner != null && a.Winner != team1) / countteamRecentGame1_5;
                double indTieRatio1_5 = countteamRecentGame1_5 == 0 ? 0 : teamRecentGame1_5.Count(a => a.Winner == null) / countteamRecentGame1_5;

                var teamRecentGame1_10 = team1.Games.Where(a => a.Date < game.Date).OrderByDescending(a => a.Date).Take(10).ToList();
                double countteamRecentGame1_10 = teamRecentGame1_10.Count();
                double indVictoRatioTeam1_10 = countteamRecentGame1_10 == 0 ? 0 : teamRecentGame1_10.Count(a => a.Winner == team1) / countteamRecentGame1_10;
                double indLooseRatioTeam1_10 = countteamRecentGame1_10 == 0 ? 0 : teamRecentGame1_10.Count(a => a.Winner != null && a.Winner != team1) / countteamRecentGame1_10;
                double indTieRatio1_10 = countteamRecentGame1_10 == 0 ? 0 : teamRecentGame1_10.Count(a => a.Winner == null) / countteamRecentGame1_10;


                var teamRecentGame2_5 = team2.Games.Where(a => a.Date < game.Date).OrderByDescending(a => a.Date).Take(5).ToList();
                double countteamRecentGame2_5 = teamRecentGame2_5.Count();
                double indVictoRatioTeam2_5 = countteamRecentGame2_5 == 0 ? 0 : teamRecentGame2_5.Count(a => a.Winner == team2) / countteamRecentGame2_5;
                double indLooseRatioTeam2_5 = countteamRecentGame2_5 == 0 ? 0 : teamRecentGame2_5.Count(a => a.Winner != null && a.Winner != team2) / countteamRecentGame2_5;
                double indTieRatio2_5 = countteamRecentGame2_5 == 0 ? 0 : teamRecentGame2_5.Count(a => a.Winner == null) / countteamRecentGame2_5;

                var teamRecentGame2_10 = team2.Games.Where(a => a.Date < game.Date).OrderByDescending(a => a.Date).Take(10).ToList();
                double countteamRecentGame2_10 = teamRecentGame2_10.Count();
                double indVictoRatioTeam2_10 = countteamRecentGame2_10 == 0 ? 0 : teamRecentGame2_10.Count(a => a.Winner == team2) / countteamRecentGame2_10;
                double indLooseRatioTeam2_10 = countteamRecentGame2_10 == 0 ? 0 : teamRecentGame2_10.Count(a => a.Winner != null && a.Winner != team2) / countteamRecentGame2_10;
                double indTieRatio2_10 = countteamRecentGame2_10 == 0 ? 0 : teamRecentGame2_10.Count(a => a.Winner == null) / countteamRecentGame2_10;





                double score1 = (confVictoRatioTeam1_10 + confVictoRatioTeam1_5 + confVictoRatioTeam1_2) / 3 + (indVictoRatioTeam1_5 + indVictoRatioTeam1_10) / 2;
                double score2 = (confVictoRatioTeam2_10 + confVictoRatioTeam2_5 + confVictoRatioTeam2_2) / 3 + (indVictoRatioTeam2_5 + indVictoRatioTeam2_10) / 2;

                Result predResult;

                Console.WriteLine(Math.Abs(score1 - score2));

                if (Math.Abs(score1 - score2) > 0.5)
                {
                    if (score1 + 0.5 > score2)
                        predResult = Result.T1;
                    else
                        predResult = Result.T2;
                    predResults.Add(new Tuple<Result, Result>(predResult, game.Result));
                }
            }

            Console.WriteLine(predResults.Count);
            Console.WriteLine(predResults.Count(a => a.Item1 == a.Item2) / (double)predResults.Count);
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
                List<RankClass> localRanks = new List<RankClass>();
                foreach (var team in Teams)
                {
                    var points = scoreTeamsHistory[team].Where(a => a.Key < date).OrderByDescending(a => a.Key).Take(gameCount).Sum(a => a.Value); ;
                    RankClass rank = new RankClass
                    {
                        GameCount = gameCount,
                        Date = date,
                        Team = team,
                        Points = points
                    };
                    team.RankHistory.Add(rank);
                }
                int pos = 1;
                localRanks.OrderByDescending(a => a.Points).ToList().ForEach(a => a.Rank = pos++);
            }
        }


    }

    public class Team
    {
        public String Name { get; set; }
        public List<Game> Games { get; set; }

        public List<RankClass> RankHistory { get; set; }


        public Team()
        {
            Games = new List<Game>();
            RankHistory = new List<RankClass>();
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



    public class RankClass
    {
        public int GameCount { get; set; }
        public int Rank { get; set; }
        public Team Team { get; set; }
        public DateTime Date { get; set; }

        public int Points { get; set; }
    }

}
