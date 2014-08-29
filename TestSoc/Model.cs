using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;

namespace TestSoc
{
    public class Model
    {
        public List<Team> Teams { get; set; }
        public List<Game> Games { get; set; }

        public Model()
        {
            Teams = new List<Team>();
            Games = new List<Game>();
        }

        private void AddGames(string file, int year)
        {
            var excel = new ExcelQueryFactory(file);
            var games = from c in excel.Worksheet<ExcelGame>(String.Format("{0}_{1}", year, year + 1))
                        select c;


            foreach (var game in games)
            {
                AddGame(game, year);
            }
        }

        private void AddGame(ExcelGame exGame, int year)
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

        public void LoadAndProcessData(Parameters Parameters)
        {
            this.LoadData();
            this.ProcessData(Parameters);
        }

        public void LoadData()
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

        }

        public void ProcessData(Parameters Parameters)
        {

            this.Teams.ForEach(a => a.StatsHistory.Clear());
            this.Games.ForEach(a => a.Stat1 = null);
            this.Games.ForEach(a => a.Stat2 = null);
            //Re-order
            Games = Games.OrderBy(a => a.Date).ToList();

            //--------------- Score ----------------------------------
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

            //int gameCount = Parameters.GameCount;
            int gameCount = (int)TestSoc.Quote.Singleton.Instance.Limit;

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
                                            .Sum(a => (a.Value == TeamResult.Win) ? 2 : ((a.Value == TeamResult.Loose) ? 0 : 1));

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
            }

            double dataMin = Teams.SelectMany(a => a.StatsHistory).Min(a => a.Points);
            double dataMax = Teams.SelectMany(a => a.StatsHistory).Max(a => a.Points);
            Teams.SelectMany(a => a.StatsHistory).ToList().ForEach(a => a.Points = NormalizeData(a.Points, dataMin, dataMax, -1, 1));


            //--------------- Weighted prob ---------------------------------- 
            foreach (var date in dates)
            {
                foreach (var team in Teams)
                {
                    var rank = team.StatsHistory.Where(a => a.Date == date).First();

                    var games = team.Games.Where(a => a.Date < date && a.Stat1 != null).OrderByDescending(a => a.Date).Take(gameCount).OrderBy(a => a.Date).ToList();
                    var homeGames = games.Where(a => a.Team1 == team).OrderBy(a => a.Date).ToList();
                    var awayGames = games.Where(a => a.Team2 == team).OrderBy(a => a.Date).ToList();

                    if (games.Count > 0)
                    {
                        double coeff = 0;
                        for (int i = 0; i < games.Count; i++)
                        {
                            var game = games[i];
                            var stat = game.GetStat(team);
                            var statOposite = game.GetOpositeStat(team);

                            var opponentWinFactor = Parameters.EnableStrongWeakOpposite ? 1 + statOposite.Points : 1;
                            var opponentLooseFactor = 2 - opponentWinFactor;

                            if (game.Result == GameResult.Tie)
                                rank.ProbTie += Parameters.Function.Y(i);

                            else if (game.Winner == team)
                                rank.ProbWin += Parameters.Function.Y(i) * opponentWinFactor;

                            else if (game.Winner != null && game.Winner != team)
                                rank.ProbLoose += Parameters.Function.Y(i) * opponentLooseFactor;

                            if (game.GetOpositeGoal(team) == 0)
                                rank.ProbClearSheet += Parameters.Function.Y(i);

                            rank.ProbGoal += game.GetGoal(team) * Parameters.Function.Y(i);
                            rank.ProbOpositeGoal += game.GetOpositeGoal(team) * Parameters.Function.Y(i);
                            rank.ProbDiffGoal += (game.GetGoal(team) - game.GetOpositeGoal(team)) * Parameters.Function.Y(i);

                            coeff += Parameters.Function.Y(i);
                        }

                        rank.ProbTie = coeff == 0 ? 0 : rank.ProbTie / coeff;
                        rank.ProbWin = coeff == 0 ? 0 : rank.ProbWin / coeff;
                        rank.ProbLoose = coeff == 0 ? 0 : rank.ProbLoose / coeff;
                        rank.ProbClearSheet = coeff == 0 ? 0 : rank.ProbClearSheet / coeff;
                        rank.ProbGoal = coeff == 0 ? 0 : rank.ProbGoal / coeff;
                        rank.ProbOpositeGoal = coeff == 0 ? 0 : rank.ProbOpositeGoal / coeff;
                        rank.ProbDiffGoal = coeff == 0 ? 0 : rank.ProbDiffGoal / coeff;
                    }

                    if (homeGames.Count > 0)
                    {
                        double coeff = 0;
                        for (int i = 0; i < homeGames.Count; i++)
                        {
                            var game = homeGames[i];
                            var stat = game.GetStat(team);
                            var statOposite = game.GetOpositeStat(team);

                            var opponentWinFactor = Parameters.EnableStrongWeakOpposite ? 1 + statOposite.Points : 1;
                            var opponentLooseFactor = 2 - opponentWinFactor;

                            if (game.Result == GameResult.Tie)
                                rank.ProbHomeTie += Parameters.Function.Y(i);

                            else if (game.Winner == team)
                                rank.ProbHomeWin += Parameters.Function.Y(i) * opponentWinFactor;

                            else if (game.Winner != null && game.Winner != team)
                                rank.ProbHomeLoose += Parameters.Function.Y(i) * opponentLooseFactor;

                            if (game.GetOpositeGoal(team) == 0)
                                rank.ProbHomeClearSheet += Parameters.Function.Y(i);

                            rank.ProbHomeGoal += game.GetGoal(team) * Parameters.Function.Y(i);
                            rank.ProbHomeOpositeGoal += game.GetOpositeGoal(team) * Parameters.Function.Y(i);
                            rank.ProbHomeDiffGoal += (game.GetGoal(team) - game.GetOpositeGoal(team)) * Parameters.Function.Y(i);

                            coeff += Parameters.Function.Y(i);
                        }

                        rank.ProbHomeTie = coeff == 0 ? 0 : rank.ProbHomeTie / coeff;
                        rank.ProbHomeWin = coeff == 0 ? 0 : rank.ProbHomeWin / coeff;
                        rank.ProbHomeLoose = coeff == 0 ? 0 : rank.ProbHomeLoose / coeff;
                        rank.ProbHomeClearSheet = coeff == 0 ? 0 : rank.ProbHomeClearSheet / coeff;
                        rank.ProbHomeGoal = coeff == 0 ? 0 : rank.ProbHomeGoal / coeff;
                        rank.ProbHomeOpositeGoal = coeff == 0 ? 0 : rank.ProbHomeOpositeGoal / coeff;
                        rank.ProbHomeDiffGoal = coeff == 0 ? 0 : rank.ProbHomeDiffGoal / coeff;
                    }

                    if (awayGames.Count > 0)
                    {
                        double coeff = 0;
                        for (int i = 0; i < awayGames.Count; i++)
                        {
                            var game = awayGames[i];
                            var stat = game.GetStat(team);
                            var statOposite = game.GetOpositeStat(team);

                            var opponentWinFactor = Parameters.EnableStrongWeakOpposite ? 1 + statOposite.Points : 1;
                            var opponentLooseFactor = 2 - opponentWinFactor;

                            if (game.Result == GameResult.Tie)
                                rank.ProbExtTie += Parameters.Function.Y(i);

                            else if (game.Winner == team)
                                rank.ProbExtWin += Parameters.Function.Y(i) * opponentWinFactor;

                            else if (game.Winner != null && game.Winner != team)
                                rank.ProbExtLoose += Parameters.Function.Y(i) * opponentLooseFactor;

                            if (game.GetOpositeGoal(team) == 0)
                                rank.ProbExtClearSheet += Parameters.Function.Y(i);

                            rank.ProbExtGoal += game.GetGoal(team) * Parameters.Function.Y(i);
                            rank.ProbExtOpositeGoal += game.GetOpositeGoal(team) * Parameters.Function.Y(i);
                            rank.ProbExtDiffGoal += (game.GetGoal(team) - game.GetOpositeGoal(team)) * Parameters.Function.Y(i);

                            coeff += Parameters.Function.Y(i);
                        }

                        rank.ProbExtTie = coeff == 0 ? 0 : rank.ProbExtTie / coeff;
                        rank.ProbExtWin = coeff == 0 ? 0 : rank.ProbExtWin / coeff;
                        rank.ProbExtLoose = coeff == 0 ? 0 : rank.ProbExtLoose / coeff;
                        rank.ProbExtClearSheet = coeff == 0 ? 0 : rank.ProbExtClearSheet / coeff;
                        rank.ProbExtGoal = coeff == 0 ? 0 : rank.ProbExtGoal / coeff;
                        rank.ProbExtOpositeGoal = coeff == 0 ? 0 : rank.ProbExtOpositeGoal / coeff;
                        rank.ProbExtDiffGoal = coeff == 0 ? 0 : rank.ProbExtDiffGoal / coeff;

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
