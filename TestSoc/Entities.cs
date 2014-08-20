using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSoc
{
    public enum GameResult
    {
        T1, T2, Tie

    }

    public enum TeamResult
    {
        Win, Loose, Tie, None

    }

    public class ExcelGame
    {
        public String Date { get; set; }
        public String Team1 { get; set; }
        public String Team2 { get; set; }
        public String Score { get; set; }

    }
    public class Team
    {
        public String Name { get; set; }
        public List<Game> Games { get; set; }

        public List<TeamStats> StatsHistory { get; set; }

        public List<int> YearsPlayed { get; set; }

        public Team()
        {
            Games = new List<Game>();
            StatsHistory = new List<TeamStats>();
            YearsPlayed = new List<int>();
        }

        internal void PlayThisYear(int year)
        {
            if (!YearsPlayed.Contains(year))
                YearsPlayed.Add(year);
        }

        public bool HavePlayThatDateTime(DateTime date)
        {
            if (date.Month > 6)
                return YearsPlayed.Contains(date.Year);
            else 
                return YearsPlayed.Contains(date.Year - 1);
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



        public GameResult Result
        {
            get
            {

                if (Winner == Team1)
                    return TestSoc.GameResult.T1;
                else if (Winner == Team2)
                    return TestSoc.GameResult.T2;

                return TestSoc.GameResult.Tie;
            }
        }


        public TeamStats Stat1 { get; set; }

        public TeamStats Stat2 { get; set; }

        public Team GetOpositeTeam(Team team)
        {
            if (Team1 == team)
                return Team2;

            if (Team2 == team)
                return Team1;

            return null;
        }

        public TeamStats GetOpositeStat(Team team)
        {
            if (Team1 == team)
                return Stat2;

            if (Team2 == team)
                return Stat1;

            return null;
        }

        public TeamStats GetStat(Team team)
        {
            if (Team1 == team)
                return Stat1;

            if (Team2 == team)
                return Stat2;

            return null;
        }

    }

    public class TeamStats
    {
        public int GameCount { get; set; }
        //public double Rank { get; set; }
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

        public Game Game { get; set; }
    }

    public class Parameters
    {
        public IFunction Function { get; set; }
        public int GameCount { get; set; }
        public bool EnableStrongWeakOpposite { get; set; }

        public Parameters()
        {
            EnableStrongWeakOpposite = true;
        }
    }
}
