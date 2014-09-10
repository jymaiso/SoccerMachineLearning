using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSoc
{
    public enum GameResult
    {
        HomeWin, AwayWin, Draw

    }

    public enum TeamResult
    {
        Win, Loose, Tie, None

    }


    public class Team
    {
        public String Name { get; set; }
        public List<Game> Games { get; set; }

        public List<TeamStats> StatsHistory { get; set; }

        public List<int> SeasonsPlayed { get; set; }

        public Team()
        {
            Games = new List<Game>();
            StatsHistory = new List<TeamStats>();
            SeasonsPlayed = new List<int>();
        }

        internal void PlayThisSeason(int year)
        {
            if (!SeasonsPlayed.Contains(year))
                SeasonsPlayed.Add(year);
        }

        public bool HavePlayThatSeason(DateTime date)
        {
            bool hasPlayed = false;
            if (date.Month > 6)
                hasPlayed = SeasonsPlayed.Contains(date.Year);
            else
                hasPlayed = SeasonsPlayed.Contains(date.Year - 1);

            return hasPlayed;
        }

        public override string ToString()
        {
            return String.Format("{0}", Name);
        }
    }

    public class Game
    {

        #region Properties
        public string Div { get; set; }
        public DateTime Date { get; set; }

        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }

        public Team Winner
        {
            get
            {
                if (_Winner == null)
                {
                    switch (FTR)
                    {
                        case GameResult.HomeWin: return HomeTeam;
                        case GameResult.AwayWin: return AwayTeam;
                        default: return null;
                    }
                }
                return _Winner;
            }
        }
        private Team _Winner = null;

        public GameResult FTR
        {
            get
            {
                if (_FTR == null)
                {
                    if (FTHG > FTAG)
                        _FTR = TestSoc.GameResult.HomeWin;
                    else if (FTHG < FTAG)
                        _FTR = TestSoc.GameResult.AwayWin;
                    else if (FTHG == FTAG)
                        _FTR = TestSoc.GameResult.Draw;
                    else
                    {
                        throw new Exception();
                    }

                }
                return (GameResult)_FTR;
            }
        }
        private GameResult? _FTR = null;

        public int FTHG { get; set; }
        public int FTAG { get; set; }

        public int HTHG { get; set; }
        public int HTAG { get; set; }

        public GameResult HTR
        {
            get
            {
                if (_HTR == null)
                {
                    if (HTHG > HTAG)
                        return TestSoc.GameResult.HomeWin;
                    else if (HTHG < HTAG)
                        return TestSoc.GameResult.AwayWin;

                    return TestSoc.GameResult.Draw;
                }
                return (GameResult)_HTR;
            }
        }
        private GameResult? _HTR = null;

        public int HS { get; set; }
        public int AS { get; set; }
        public int HST { get; set; }
        public int AST { get; set; }
        public int HF { get; set; }
        public int AF { get; set; }
        public int HC { get; set; }
        public int AC { get; set; }
        public int HY { get; set; }
        public int AY { get; set; }
        public int HR { get; set; }
        public int AR { get; set; }

        public int Home_Score
        {
            get
            {
                if (_Home_Score == null)
                {
                    if (FTHG > FTAG)
                        _Home_Score = 1;
                    else if (FTHG < FTAG)
                        _Home_Score = -1;
                    else if (FTHG == FTAG)
                        _Home_Score = 0;
                    else
                    {
                        throw new Exception();
                    }
                }
                return (int)_Home_Score;
            }
        }
        private int? _Home_Score;



        public int Home_Win { get { return (FTR == GameResult.HomeWin) ? 1 : 0; } }
        public int Home_Loose { get { return (FTR == GameResult.AwayWin) ? 1 : 0; } }
        public int Home_Draw { get { return (FTR == GameResult.Draw) ? 1 : 0; } }

        public int Away_Score
        {
            get
            {
                if (_Away_Score == null)
                {
                    if (FTHG < FTAG)
                        _Away_Score = 1;
                    else if (FTHG > FTAG)
                        _Away_Score = -1;
                    else if (FTHG == FTAG)
                        _Away_Score = 0;
                    else
                    {
                        throw new Exception();
                    }
                }
                return (int)_Away_Score;
            }
        }
        private int? _Away_Score;
        public int Away_Win { get { return (FTR == GameResult.AwayWin) ? 1 : 0; } }
        public int Away_Loose { get { return (FTR == GameResult.HomeWin) ? 1 : 0; } }
        public int Away_Draw { get { return (FTR == GameResult.Draw) ? 1 : 0; } }

        public TeamStats HomeStat { get; set; }
        public TeamStats AwayStat { get; set; }
        #endregion



        public Team GetOpositeTeam(Team team)
        {
            if (HomeTeam == team)
                return AwayTeam;

            if (AwayTeam == team)
                return HomeTeam;

            return null;
        }

        public TeamStats GetOpositeStat(Team team)
        {
            if (HomeTeam == team)
                return AwayStat;

            if (AwayTeam == team)
                return HomeStat;

            return null;
        }

        public TeamStats GetStat(Team team)
        {
            if (HomeTeam == team)
                return HomeStat;

            if (AwayTeam == team)
                return AwayStat;

            throw new Exception();
        }

        public int GetGoal(Team team)
        {
            if (HomeTeam == team)
                return FTHG;

            if (AwayTeam == team)
                return FTAG;

            throw new Exception();
        }

        public int GetOpositeGoal(Team team)
        {
            if (HomeTeam == team)
                return FTAG;

            if (AwayTeam == team)
                return FTHG;

            throw new Exception();
        }


        public override string ToString()
        {
            return String.Format("{0} {1} {2}", Date.ToShortDateString(), HomeTeam.Name, AwayTeam.Name);
        }
    }

    public class TeamStats
    {
        public Team Team { get; set; }
        public DateTime Date { get; set; }
        public Game Game { get; set; }

        public double JPoints { get; set; }

        public double Home_Win { get; set; }
        public double Home_Loose { get; set; }
        public double Home_Draw { get; set; }
        public double Home_FTHG { get; set; }
        public double Home_FTAG { get; set; }
        public double Home_HTHG { get; set; }
        public double Home_HTAG { get; set; }
        public double Home_HS { get; set; }
        public double Home_AS { get; set; }
        public double Home_HST { get; set; }
        public double Home_AST { get; set; }
        public double Home_HF { get; set; }
        public double Home_AF { get; set; }
        public double Home_HC { get; set; }
        public double Home_AC { get; set; }
        public double Home_HY { get; set; }
        public double Home_AY { get; set; }
        public double Home_HR { get; set; }
        public double Home_AR { get; set; }

        public double Away_Win { get; set; }
        public double Away_Loose { get; set; }
        public double Away_Draw { get; set; }
        public double Away_FTHG { get; set; }
        public double Away_FTAG { get; set; }
        public double Away_HTHG { get; set; }
        public double Away_HTAG { get; set; }
        public double Away_HS { get; set; }
        public double Away_AS { get; set; }
        public double Away_HST { get; set; }
        public double Away_AST { get; set; }
        public double Away_HF { get; set; }
        public double Away_AF { get; set; }
        public double Away_HC { get; set; }
        public double Away_AC { get; set; }
        public double Away_HY { get; set; }
        public double Away_AY { get; set; }
        public double Away_HR { get; set; }
        public double Away_AR { get; set; }


        public TeamStats()
        {
            JPoints = 2000;
        }


        public override string ToString()
        {
            return String.Format("{0} {1}", Date.ToShortDateString(), Team.Name);
        }

        public double HomePoints { get; set; }

        public double AwayPoints { get; set; }
    }

    public class Parameters
    {
        public static double DrawRatio { get; set; }

        public IFunction Function { get; set; }
        public int GameCount { get; set; }

        public Parameters()
        {
            x0 = 0.1;
            x1 = 0.1;
            x2 = 0.1;
            x3 = 0.1;
            x4 = 0.1;
            k = 0.1;
        }

        public double x0 { get; set; }

        public double x1 { get; set; }

        public double x2 { get; set; }

        public double x3 { get; set; }

        public double x4 { get; set; }

        public double k { get; set; }
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

        //private double P1 { get { return (Stat1.Home_Win + Stat2.Away_Loose) / 2; } }
        //private double PT { get { return (Stat1.Home_Draw + Stat2.Away_Draw) / 2; } }
        //private double P2 { get { return (Stat2.Away_Win + Stat1.Home_Loose) / 2; } }

        private double P1 { get { return (Stat1.HomePoints); } }
        private double PT { get { return 0; } }
        private double P2 { get { return (Stat2.AwayPoints); } }

        //private double P1 { get { return (Stat1.ProbWin) ; } }
        //private double PT { get { return (Stat1.ProbTie + Stat2.ProbTie) / 2; } }
        //private double P2 { get { return (Stat2.ProbWin); } }

        private double TotalP
        {
            get
            {
                if (_TotalP == null) _TotalP = (P1 + PT + P2);
                return (double)_TotalP;
            }
        }
        private double? _TotalP;

        private double TotalQ { get { return (1 / Q1 + 1 / QT + 1 / Q2); } }

        public double MyProb1 { get { return (P1 / TotalP); } }
        public double MyProbT { get { return 0.28; } }
        public double MyProb2 { get { return (P2 / TotalP); } }

        public int ActualProb1 { get { return Game.Home_Win; } }
        public int ActualProbT { get { return Game.Home_Draw; } }
        public int ActualProb2 { get { return Game.Away_Win; } }

        public double BookieProb1 { get { return 1 / Q1 / TotalQ; } }
        public double BookieProbT { get { return 1 / QT / TotalQ; } }
        public double BookieProb2 { get { return 1 / Q2 / TotalQ; } }

        public double MyMSE { get { return 1 / (double)3 * (Math.Pow(MyProb1 - ActualProb1, 2) + Math.Pow(MyProbT - ActualProbT, 2) + Math.Pow(MyProb2 - ActualProb2, 2)); } }
        public double BookieMSE { get { return 1 / (double)3 * (Math.Pow(BookieProb1 - ActualProb1, 2) + Math.Pow(BookieProbT - ActualProbT, 2) + Math.Pow(BookieProb2 - ActualProb2, 2)); } }
        public double RandomMSE { get { return 1 / (double)3 * (Math.Pow(0.333333 - ActualProb1, 2) + Math.Pow(0.333333 - ActualProbT, 2) + Math.Pow(0.333333 - ActualProb2, 2)); } }
        public double HomeWinMSE { get { return 1 / (double)3 * (Math.Pow(0.47 - ActualProb1, 2) + Math.Pow(0.29 - ActualProbT, 2) + Math.Pow(0.24 - ActualProb2, 2)); } }

        public double MyCorrect
        {
            get
            {
                switch (MyResult)
                {
                    case GameResult.HomeWin:
                        return Game.Winner == Team1 ? 1 : 0;
                    case GameResult.AwayWin:
                        return Game.Winner == Team2 ? 1 : 0;
                    case GameResult.Draw:
                        return Game.Winner == null ? 1 : 0;
                    default:
                        break;
                }

                throw new Exception();
            }
        }

        public class Singleton
        {
            public double Limit { get; set; }

            private static Singleton instance;

            private Singleton() { }

            public static Singleton Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new Singleton();
                    }
                    return instance;
                }
            }
        }

        public GameResult MyResult
        {
            get
            {
                //if (MyProb1 >= MyProbT && MyProb1 >= MyProb2)
                //    return GameResult.T1;
                //else if (MyProbT >= MyProb1 && MyProbT >= MyProb2)
                //    return GameResult.Tie;
                //else if (MyProb2 >= MyProbT && MyProb2 >= MyProb1)
                //    return GameResult.T2;

                if (MyProb1 * 2 >= MyProb2)
                    return GameResult.HomeWin;

                else
                    return GameResult.AwayWin;

                throw new Exception();
            }
        }

        public double BookieCorrect
        {
            get
            {
                if (BookieProb1 >= BookieProbT && BookieProb1 >= BookieProb2)
                    return Game.Winner == Team1 ? 1 : 0;
                else if (BookieProbT >= BookieProb1 && BookieProbT >= BookieProb2)
                    return Game.Winner == null ? 1 : 0;
                else if (BookieProb2 >= BookieProbT && BookieProb2 >= BookieProb1)
                    return Game.Winner == Team2 ? 1 : 0;

                return 0;
            }
        }

        public GameResult BookiResult
        {
            get
            {
                if (BookieProb1 >= BookieProbT && BookieProb1 >= BookieProb2)
                    return GameResult.HomeWin;
                else if (BookieProbT >= BookieProb1 && BookieProbT >= BookieProb2)
                    return GameResult.Draw;
                else if (BookieProb2 >= BookieProbT && BookieProb2 >= BookieProb1)
                    return GameResult.AwayWin;

                throw new Exception();
            }
        }
    }

    public class BetPot
    {
        public double pot = 100;
        public int bet = 0;
        public int ttrue = 0;
        public int ffalse = 0;
    }
}
