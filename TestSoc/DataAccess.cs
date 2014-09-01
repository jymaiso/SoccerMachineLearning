using LINQtoCSV;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//http://www.football-data.co.uk
//Div = League Division
//Date = Match Date (dd/mm/yy)
//HomeTeam = Home Team
//AwayTeam = Away Team
//FTHG = Full Time Home Team Goals
//FTAG = Full Time Away Team Goals
//FTR = Full Time Result (H=Home Win, D=Draw, A=Away Win)
//HTHG = Half Time Home Team Goals
//HTAG = Half Time Away Team Goals
//HTR = Half Time Result (H=Home Win, D=Draw, A=Away Win)

//Match Statistics (where available)
//Attendance = Crowd Attendance
//Referee = Match Referee
//HS = Home Team Shots
//AS = Away Team Shots
//HST = Home Team Shots on Target
//AST = Away Team Shots on Target
//HHW = Home Team Hit Woodwork
//AHW = Away Team Hit Woodwork
//HC = Home Team Corners
//AC = Away Team Corners
//HF = Home Team Fouls Committed
//AF = Away Team Fouls Committed
//HO = Home Team Offsides
//AO = Away Team Offsides
//HY = Home Team Yellow Cards
//AY = Away Team Yellow Cards
//HR = Home Team Red Cards
//AR = Away Team Red Cards
//HBP = Home Team Bookings Points (10 = yellow, 25 = red)
//ABP = Away Team Bookings Points (10 = yellow, 25 = red)


namespace TestSoc.DataAccess
{
    interface IDataAccess
    {
        void LoadData(ModelGames model);
    }

    public class DataAccessFootballData : IDataAccess
    {
        public void LoadData(ModelGames model)
        {
            AddGames(model, 2005);
            AddGames(model, 2006);
            AddGames(model, 2007);
            AddGames(model, 2008);
            AddGames(model, 2009);
            AddGames(model, 2010);
            AddGames(model, 2011);
            AddGames(model, 2012);
            AddGames(model, 2013);
        }

        private void AddGames(ModelGames model, int year)
        {
            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true,
                IgnoreUnknownColumns = true,
                FileCultureInfo = CultureInfo.GetCultureInfo("en-GB"),

            };

            CsvContext cc = new CsvContext();

            IEnumerable<GameFootballData> games = cc.Read<GameFootballData>(Path.Combine(Environment.CurrentDirectory, "DataAccess", "FootballData", String.Format("{0}_{1}", year, year + 1) + ".csv"), inputFileDescription);

            foreach (GameFootballData game in games)
            {
                Team team1 = model.AddOrUpdateTeam(game.HomeTeam.Trim(), year);
                Team team2 = model.AddOrUpdateTeam(game.AwayTeam.Trim(), year);

                Game newGame = new Game
                {
                    Date = game.Date,
                    HomeTeam = team1,
                    AwayTeam = team2,
                    Div = game.Div,
                    FTHG = game.FTHG,
                    FTAG = game.FTAG,
                    HTHG = game.HTHG,
                    HTAG = game.HTAG,
                    HS = game.HS,
                    AS = game.AS,
                    HST = game.HST,
                    AST = game.AST,
                    HF = game.HF,
                    AF = game.AF,
                    HC = game.HC,
                    AC = game.AC,
                    HY = game.HY,
                    AY = game.AY,
                    HR = game.HR,
                    AR = game.AR,
                };

                model.Games.Add(newGame);
                team1.Games.Add(newGame);
                team2.Games.Add(newGame);
            }
        }

        private class GameFootballData
        {
            public string Div { get; set; }
            public DateTime Date { get; set; }
            public string HomeTeam { get; set; }
            public string AwayTeam { get; set; }
            public int FTHG { get; set; }
            public int FTAG { get; set; }
            public string FTR { get; set; }
            public int HTHG { get; set; }
            public int HTAG { get; set; }
            public string HTR { get; set; }
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
        }
    }

    public class DataAccessV1 : IDataAccess
    {
        public void LoadData(ModelGames model)
        {
            AddGames(model, 2005);
            AddGames(model, 2006);
            AddGames(model, 2007);
            AddGames(model, 2008);
            AddGames(model, 2009);
            AddGames(model, 2010);
            AddGames(model, 2011);
            AddGames(model, 2012);
            AddGames(model, 2013);
        }

        private void AddGames(ModelGames model, int year)
        {
            string file = Path.Combine(Path.Combine(Environment.CurrentDirectory, "DataAccess", "V1", "data.xlsx"));

            var excel = new ExcelQueryFactory(file);
            var games = from c in excel.Worksheet<GameV1>(String.Format("{0}_{1}", year, year + 1))
                        select c;


            foreach (var game in games)
            {
                AddGame(model, game, year);
            }
        }

        private void AddGame(ModelGames model, GameV1 exGame, int year)
        {
            var arr = exGame.Date.Split('.');
            var dayAndMonth = new { day = Convert.ToInt32(arr[0]), month = Convert.ToInt32(arr[1]) };

            DateTime date = (dayAndMonth.month > 6) ? new DateTime(year, dayAndMonth.month, dayAndMonth.day) : new DateTime(year + 1, dayAndMonth.month, dayAndMonth.day);

            int indexOfSeparator = exGame.Score.IndexOf(":");
            var Score1 = Convert.ToInt32(exGame.Score.Substring(0, indexOfSeparator).Trim());
            var Score2 = Convert.ToInt32(exGame.Score.Substring(indexOfSeparator + 1, exGame.Score.Length - indexOfSeparator - 1).Trim());

            Team team1 = model.AddOrUpdateTeam(exGame.Team1.Trim(), year);
            Team team2 = model.AddOrUpdateTeam(exGame.Team2.Trim(), year);

            Game newGame = new Game
            {
                Date = date,
                HomeTeam = team1,
                AwayTeam = team2,
                FTHG = Score1,
                FTAG = Score2,
            };

            model.Games.Add(newGame);
            team1.Games.Add(newGame);
            team2.Games.Add(newGame);
        }

        class GameV1
        {
            public String Date { get; set; }
            public String Team1 { get; set; }
            public String Team2 { get; set; }
            public String Score { get; set; }

        }
    }

    public static class CSVFactory
    {
        public static void GetCSV(ModelGames model, String fileName = null)
        {
            List<Game> Games = model.Games;

            Type type = typeof(TeamStats);
            List<PropertyInfo> properties = type.GetProperties().Where(a => a.CanRead && a.PropertyType == typeof(double)).ToList();
            var sb = new StringBuilder();

            // First line contains field names

            sb.Append(String.Join(",", properties.Select(a => a.Name + "_1").Union(properties.Select(a => a.Name + "_2"))));
            sb.Append(",Output");
            sb.AppendLine();

            var games = Games.Where(a => a.Date.Year >= 2007).ToList().Shuffle();
            var games1 = games.Where(a => a.FTR == GameResult.HomeWin).Take(740).ToList().Shuffle();
            var gamesT = games.Where(a => a.FTR == GameResult.Draw).Take(740).ToList().Shuffle();
            var games2 = games.Where(a => a.FTR == GameResult.AwayWin).Take(740).ToList().Shuffle();

            games = games1.Union(gamesT).Union(games2).ToList().Shuffle();

            foreach (Game game in games)
            {

                foreach (PropertyInfo prp in properties)
                {
                    if (prp.CanRead)
                    {
                        sb.Append(prp.GetValue(game.HomeStat, null).ToString().Replace(",", ".")).Append(',');
                    }
                }

                foreach (PropertyInfo prp in properties)
                {
                    if (prp.CanRead)
                    {
                        sb.Append(prp.GetValue(game.AwayStat, null).ToString().Replace(",", ".")).Append(',');
                    }
                }

                sb.Append(game.FTR);
                sb.AppendLine();
            }

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName == null ? "data.csv" : fileName + ".csv");
            File.WriteAllText(file, sb.ToString());
            //Process.Start(file);
        }

    }
}
