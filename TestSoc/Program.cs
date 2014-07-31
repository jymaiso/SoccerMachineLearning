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
    class Program
    {
        static void Main(string[] args)
        {
            string file = Path.Combine(Environment.CurrentDirectory, "data.xlsx");


            AddGames(file, 1998);
            AddGames(file, 1999);
            AddGames(file, 2000);
            AddGames(file, 2001);
            AddGames(file, 2002);
            AddGames(file, 2003);
            AddGames(file, 2004);
            AddGames(file, 2005);
            AddGames(file, 2006);
            AddGames(file, 2007);
            AddGames(file, 2008);
            AddGames(file, 2009);
            AddGames(file, 2010);
            AddGames(file, 2011);
            AddGames(file, 2012);
            AddGames(file, 2013);
            Cache.Instance.Games = Cache.Instance.Games.OrderBy(a => a.Date).ToList();




            RunSimpleExport();
            //RunSimplePrediction();

            Console.ReadKey();

        }

        private static void RunSimpleExport()
        {
            StringBuilder sb = new StringBuilder();

            List<String> labels = new List<string>(){
                    "confVictoRatioTeam1_10",
                    "confVictoRatioTeam2_10",
                    "confTieRatio_10",
                    "confVictoRatioTeam1_5",
                    "confVictoRatioTeam2_5",
                    "confTieRatio_5",
                    "confVictoRatioTeam1_2",
                    "confVictoRatioTeam2_2",
                    "confTieRatio_2",
                    
                    "indVictoRatioTeam1_5",
                    "indLooseRatioTeam1_5",
                    "indTieRatio1_5",
                    
                    "indVictoRatioTeam1_10",
                    "indLooseRatioTeam1_10",
                    "indTieRatio1_10",
                    
                    "indVictoRatioTeam2_5",
                    "indLooseRatioTeam2_5",
                    "indTieRatio2_5",
                    
                    "indVictoRatioTeam2_10",
                    "indLooseRatioTeam2_10",
                    "indTieRatio2_10",
                    
                    "game.Result",
            };

            sb.AppendLine(String.Join(",", labels));

            for (int i = 0; i < Cache.Instance.Games.Count - 1000; i++)
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







                List<String> entry = new List<String>(){
                    confVictoRatioTeam1_10.ToString().Replace(",","."),
                    confVictoRatioTeam2_10.ToString().Replace(",","."),
                    confTieRatio_10.ToString().Replace(",","."),
                    
                    confVictoRatioTeam1_5.ToString().Replace(",","."),
                    confVictoRatioTeam2_5.ToString().Replace(",","."),
                    confTieRatio_5.ToString().Replace(",","."),
                    
                    confVictoRatioTeam1_2.ToString().Replace(",","."),
                    confVictoRatioTeam2_2.ToString().Replace(",","."),
                    confTieRatio_2.ToString().Replace(",","."),

                    indVictoRatioTeam1_5.ToString().Replace(",","."),
                    indLooseRatioTeam1_5.ToString().Replace(",","."),
                    indTieRatio1_5.ToString().Replace(",","."),
                   
                    indVictoRatioTeam1_10.ToString().Replace(",","."),
                    indLooseRatioTeam1_10.ToString().Replace(",","."),
                    indTieRatio1_10.ToString().Replace(",","."),
                    
                    indVictoRatioTeam2_5.ToString().Replace(",","."),
                    indLooseRatioTeam2_5.ToString().Replace(",","."),
                    indTieRatio2_5.ToString().Replace(",","."),
                    
                    indVictoRatioTeam2_10.ToString().Replace(",","."),
                    indLooseRatioTeam2_10.ToString().Replace(",","."),
                    indTieRatio2_10.ToString().Replace(",","."),
                    
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
                var recentConfronts = confronts.OrderByDescending(a => a.Date).Take(3);
                double countConfront = recentConfronts.Count();

                double confVictoRatioTeam1 = countConfront == 0 ? 0 : recentConfronts.Count(a => a.Winner == team1) / countConfront;
                double confVictoRatioTeam2 = countConfront == 0 ? 0 : recentConfronts.Count(a => a.Winner == team2) / countConfront;
                double confTieRatio = countConfront == 0 ? 0 : recentConfronts.Count(a => a.Winner == null) / countConfront;


                var teamRecentGame1 = team1.Games.Where(a => a.Date < game.Date).OrderByDescending(a => a.Date).Take(10).ToList();
                double countteamRecentGame1 = teamRecentGame1.Count();
                var teamRecentGame2 = team2.Games.Where(a => a.Date < game.Date).OrderByDescending(a => a.Date).Take(10).ToList();
                double countteamRecentGame2 = teamRecentGame2.Count();

                double indVictoRatioTeam1 = countteamRecentGame1 == 0 ? 0 : teamRecentGame1.Count(a => a.Winner == team1) / countteamRecentGame1;
                double indLooseRatioTeam1 = countteamRecentGame1 == 0 ? 0 : teamRecentGame1.Count(a => a.Winner != null && a.Winner != team1) / countteamRecentGame1;
                double indTieRatio1 = countteamRecentGame1 == 0 ? 0 : teamRecentGame1.Count(a => a.Winner == null) / countteamRecentGame1;

                double indVictoRatioTeam2 = countteamRecentGame2 == 0 ? 0 : teamRecentGame2.Count(a => a.Winner == team2) / countteamRecentGame2;
                double indLooseRatioTeam2 = countteamRecentGame2 == 0 ? 0 : teamRecentGame2.Count(a => a.Winner != null && a.Winner != team2) / countteamRecentGame2;
                double indTieRatio2 = countteamRecentGame2 == 0 ? 0 : teamRecentGame2.Count(a => a.Winner == null) / countteamRecentGame2;

                double score1 = confVictoRatioTeam1 / 2 + indVictoRatioTeam1 - indLooseRatioTeam1;
                double score2 = confVictoRatioTeam2 / 2 + indVictoRatioTeam2 - indLooseRatioTeam2;
                double tieScore = confTieRatio + indTieRatio1 + indTieRatio2;

                Result predResult;
                //Console.WriteLine(Math.Abs(score1 - score2));
                if (Math.Abs(score1 - score2) < 0.5 && tieScore > 0.9)
                    predResult = Result.Tie;
                else
                    if (score1 - score2 > -0.2)
                        predResult = Result.T1;
                    else
                        predResult = Result.T2;

                predResults.Add(new Tuple<Result, Result>(predResult, game.Result));
            }

            var tieRatio = Cache.Instance.Games.Where(a => a.Result == Result.Tie).Count() / (double)Cache.Instance.Games.Count;
            var predTieRatio = predResults.Count(a => a.Item1 == Result.Tie) / (double)predResults.Count;


            Console.WriteLine(predResults.Count(a => a.Item1 == a.Item2) / (double)predResults.Count);
        }

        private static void AddGames(string file, int year, bool addGameToTeam = true)
        {
            var excel = new ExcelQueryFactory(file);
            var games = from c in excel.Worksheet<ExcelGame>(String.Format("{0}_{1}", year, year + 1))
                        select c;


            foreach (var game in games)
            {
                Cache.Instance.AddGame(game, year);
            }


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
    }

    public class Team
    {
        public String Name { get; set; }
        public List<Game> Games { get; set; }

        public Team()
        {
            Games = new List<Game>();
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
}
