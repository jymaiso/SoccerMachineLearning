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

            throw new Exception();
        }

        public int GetGoal(Team team)
        {
            if (Team1 == team)
                return Score1;

            if (Team2 == team)
                return Score2;

            throw new Exception();
        }

        public int GetOpositeGoal(Team team)
        {
            if (Team1 == team)
                return Score2;

            if (Team2 == team)
                return Score1;

            throw new Exception();
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

        public double ProbClearSheet { get; set; }

        public double ProbGoal { get; set; }

        public double ProbOpositeGoal { get; set; }

        public double ProbDiffGoal { get; set; }

        public double ProbHomeClearSheet { get; set; }

        public double ProbHomeGoal { get; set; }

        public double ProbHomeOpositeGoal { get; set; }

        public double ProbHomeDiffGoal { get; set; }

        public double ProbExtClearSheet { get; set; }

        public double ProbExtGoal { get; set; }

        public double ProbExtOpositeGoal { get; set; }

        public double ProbExtDiffGoal { get; set; }


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

    public class ExcelQuote
    {
        public String Date { get; set; }
        public String Teams { get; set; }
        public String Q1 { get; set; }
        public String QT { get; set; }
        public String Q2 { get; set; }
    }

    public class Quote
    {
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }

        public double Q1 { get; set; }
        public double QT { get; set; }
        public double Q2 { get; set; }



        public DateTime Date { get; set; }

        public Game Game { get; set; }


        public TeamStats Stat1
        {
            get
            {
                if (_s1 == null)
                {
                    _s1 = Team1.StatsHistory.Where(a => a.Date == Date).First();
                }
                return _s1;
            }
            set { _s1 = value; }
        }
        private TeamStats _s1;

        public TeamStats Stat2
        {
            get
            {
                if (_s2 == null)
                {
                    _s2 = Team2.StatsHistory.Where(a => a.Date == Date).First();
                }
                return _s2;
            }
            set { _s2 = value; }
        }
        private TeamStats _s2;

        private double P1 { get { return (Stat1.ProbHomeWin + Stat2.ProbExtLoose) / 2; } }
        private double PT { get { return (Stat1.ProbHomeTie + Stat2.ProbExtTie) / 2; } }
        private double P2 { get { return (Stat2.ProbExtWin + Stat1.ProbHomeLoose) / 2; } }
        private double TotalP { get { return (P1 + PT + P2); } }

        private double TotalQ { get { return (1 / Q1 + 1 / QT + 1 / Q2); } }

        public double MyProb1 { get { return P1 / TotalP; } }
        public double MyProbT { get { return PT / TotalP; } }
        public double MyProb2 { get { return P2 / TotalP; } }

        public int ActualProb1 { get { return Game.Result == GameResult.T1 ? 1 : 0; } }
        public int ActualProbT { get { return Game.Result == GameResult.Tie ? 1 : 0; } }
        public int ActualProb2 { get { return Game.Result == GameResult.T2 ? 1 : 0; } }

        public double BookieProb1 { get { return 1 / Q1 / TotalQ; } }
        public double BookieProbT { get { return 1 / QT / TotalQ; } }
        public double BookieProb2 { get { return 1 / Q2 / TotalQ; } }

        public double MyMSE { get { return 1 / (double)3 * (Math.Pow(MyProb1 - ActualProb1, 2) + Math.Pow(MyProbT - ActualProbT, 2) + Math.Pow(MyProb2 - ActualProb2, 2)); } }
        public double BookieMSE { get { return 1 / (double)3 * (Math.Pow(BookieProb1 - ActualProb1, 2) + Math.Pow(BookieProbT - ActualProbT, 2) + Math.Pow(BookieProb2 - ActualProb2, 2)); } }
        public double RandomMSE { get { return 1 / (double)3 * (Math.Pow(0.333333 - ActualProb1, 2) + Math.Pow(0.333333 - ActualProbT, 2) + Math.Pow(0.333333 - ActualProb2, 2)); } }
    }

    public class BetPot
    {
        public double pot = 100;
        public int bet = 0;
        public int ttrue = 0;
        public int ffalse = 0;
    }
}
