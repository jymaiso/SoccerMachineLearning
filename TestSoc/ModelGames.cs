using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;
using TestSoc.DataAccess;

namespace TestSoc
{
    public class ModelGames
    {
        public List<Team> Teams { get; set; }
        public List<Game> Games { get; set; }

        public ModelGames()
        {
            Teams = new List<Team>();
            Games = new List<Game>();

            IDataAccess da = new DataAccessFootballData();
            da.LoadData(this);
        }

        public Team AddOrUpdateTeam(string teamName, int year)
        {
            Team team1 = Teams.Where(a => a.Name.ToLower() == teamName.ToLower()).FirstOrDefault();
            if (team1 == null)
            {
                team1 = new Team { Name = teamName };
                Teams.Add(team1);
            }
            team1.PlayThisSeason(year);
            return team1;
        }

        public void ProcessData(Parameters Parameters)
        {
            //Clear data
            this.Teams.ForEach(a => a.StatsHistory.Clear());
            this.Games.ForEach(a => a.HomeStat = null);
            this.Games.ForEach(a => a.AwayStat = null);

            //Re-order games
            Games = Games.OrderBy(a => a.Date).ToList();

            //--------------- Weighted prob ---------------------------------- 
            foreach (var team in Teams)
            {
                foreach (var game in team.Games)
                {
                    TeamStats stat = AddStats(game, team);

                    var games = team.Games.Where(a => a.Date < game.Date).OrderByDescending(a => a.Date).Take(Parameters.GameCount).ToList();
                    for (int i = 0; i < games.Count - 1; i++)
                        if ((games[i].Date - games[i + 1].Date).TotalDays > 365)
                        {
                            games = games.Take(i + 1).ToList();
                            break;
                        }

                    var homeGames = games.Where(a => a.HomeTeam == team).OrderBy(a => a.Date).ToList();
                    var awayGames = games.Where(a => a.AwayTeam == team).OrderBy(a => a.Date).ToList();



                    if (homeGames.Count > 0)
                    {
                        var sum = Parameters.Function.Sum(homeGames.Count);
                        homeGames.ToList().ForEach(g =>
                        {
                            var index = homeGames.IndexOf(g);
                            var y = Parameters.Function.Y(index) / sum;

                            stat.Home_Win += ((g.FTR == GameResult.HomeWin) ? 1 : 0) * y;
                            stat.Home_Loose += ((g.FTR == GameResult.AwayWin) ? 1 : 0) * y;
                            stat.Home_Draw += ((g.FTR == GameResult.Draw) ? 1 : 0) * y;
                            stat.Home_FTHG += g.FTHG * y;
                            stat.Home_FTAG += g.FTAG * y;
                            stat.Home_HTHG += g.HTHG * y;
                            stat.Home_HTAG += g.HTAG * y;
                            stat.Home_HS += g.HS * y;
                            stat.Home_AS += g.AS * y;
                            stat.Home_HST += g.HST * y;
                            stat.Home_AST += g.AST * y;
                            stat.Home_HF += g.HF * y;
                            stat.Home_AF += g.AF * y;
                            stat.Home_HC += g.HC * y;
                            stat.Home_AC += g.AC * y;
                            stat.Home_HY += g.HY * y;
                            stat.Home_AY += g.AY * y;
                            stat.Home_HR += g.HR * y;
                            stat.Home_AR += g.AR * y;
                        });
                    }

                    if (awayGames.Count > 0)
                    {
                        var sum = Parameters.Function.Sum(awayGames.Count);
                        awayGames.ToList().ForEach(g =>
                        {
                            var index = awayGames.IndexOf(g);
                            var y = Parameters.Function.Y(index) / sum;

                            stat.Away_Win += ((g.FTR == GameResult.AwayWin) ? 1 : 0) * y;
                            stat.Away_Loose += ((g.FTR == GameResult.HomeWin) ? 1 : 0) * y;
                            stat.Away_Draw += ((g.FTR == GameResult.Draw) ? 1 : 0) * y;
                            stat.Away_FTHG += g.FTHG * y;
                            stat.Away_FTAG += g.FTAG * y;
                            stat.Away_HTHG += g.HTHG * y;
                            stat.Away_HTAG += g.HTAG * y;
                            stat.Away_HS += g.HS * y;
                            stat.Away_AS += g.AS * y;
                            stat.Away_HST += g.HST * y;
                            stat.Away_AST += g.AST * y;
                            stat.Away_HF += g.HF * y;
                            stat.Away_AF += g.AF * y;
                            stat.Away_HC += g.HC * y;
                            stat.Away_AC += g.AC * y;
                            stat.Away_HY += g.HY * y;
                            stat.Away_AY += g.AY * y;
                            stat.Away_HR += g.HR * y;
                            stat.Away_AR += g.AR * y;
                        });
                    }
                }
            }


            int count = 0;

            foreach (var game in Games)
            {
                count++;
                var HomePreviousStat = game.HomeTeam.StatsHistory.Where(a => a.Date < game.Date).OrderByDescending(a => a.Date).FirstOrDefault();
                var AwayPreviousStat = game.AwayTeam.StatsHistory.Where(a => a.Date < game.Date).OrderByDescending(a => a.Date).FirstOrDefault();

                var HomePreviousPoint = HomePreviousStat == null ? 2000 : HomePreviousStat.JPoints;
                var AwayPreviousPoint = AwayPreviousStat == null ? 2000 : AwayPreviousStat.JPoints;

                var HomeStronger = HomePreviousPoint - AwayPreviousPoint;
                var AwayStronger = AwayPreviousPoint - HomePreviousPoint;

                var HomePoints = Parameters.x0 * AwayStronger + Parameters.x1 * game.Home_Score + Parameters.x2 * (game.FTHG - game.FTAG) + Parameters.x3 * game.HS - Parameters.x4 * game.AS;
                var AwayPoints = Parameters.x0 * HomeStronger + Parameters.x1 * game.Away_Score + Parameters.x2 * (game.FTAG - game.FTHG) + Parameters.x3 * game.AS - Parameters.x4 * game.HS;

                game.HomeStat.JPoints = HomePreviousPoint + Parameters.k * HomePoints;
                game.AwayStat.JPoints = AwayPreviousPoint + Parameters.k * AwayPoints;
            }
        }

        private static TeamStats AddStats(Game game, Team team)
        {
            //Create team stats
            TeamStats stat = new TeamStats
            {
                Date = game.Date,
                Team = team,
            };
            team.StatsHistory.Add(stat);

            if (game.HomeTeam == team)
                game.HomeStat = stat;
            else if (game.AwayTeam == team)
                game.AwayStat = stat;

            stat.Game = game;

            return stat;
        }
    }
}
