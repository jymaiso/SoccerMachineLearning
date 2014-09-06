using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;

namespace TestSoc
{
    public class ModelQuotes
    {
        public ModelGames _model { get; set; }

        public List<Quote> Quotes { get; set; }

        public ModelQuotes(ModelGames model)
        {
            this._model = model;
            Quotes = new List<Quote>();

            string file = Path.Combine(Environment.CurrentDirectory, "DataAccess", "V1", "quote.xlsx");
            var excel = new ExcelQueryFactory(file);
             _ExcelQuotes = (from c in excel.Worksheet<ExcelQuote>(String.Format("{0}_{1}", 2013, 2013 + 1))
                      select c).ToList();
        }

        private List<ExcelQuote> _ExcelQuotes;

        public void LoadData()
        {
            Quotes.Clear();
            this.AddQuotes( );
        }

        private void AddQuotes( )
        {
            foreach (var q in _ExcelQuotes)
            {
                AddQuote(q);
            }
        }

        private void AddQuote(ExcelQuote exQuote)
        {

            String team1Str = exQuote.Teams.Substring(0, exQuote.Teams.IndexOf("-") - 1).Trim();
            String team2Str = exQuote.Teams.Substring(exQuote.Teams.IndexOf("-") + 1, exQuote.Teams.Length - exQuote.Teams.IndexOf("-") - 1).Trim();

            Team team1 = _model.Teams.Where(a => a.Name.ToLower() == team1Str.ToLower().Trim()).FirstOrDefault();
            if (team1 == null)
            {
                team1Str = TeamNameMapping.Instance.Dic[team1Str];

                team1 = _model.Teams.Where(a => a.Name.ToLower() == team1Str.ToLower().Trim()).FirstOrDefault();
                if (team1 == null) throw new Exception("Unknown team");
            }


            Team team2 = _model.Teams.Where(a => a.Name.ToLower() == team2Str.ToLower().Trim()).FirstOrDefault();
            if (team2 == null)
            {
                team2Str = TeamNameMapping.Instance.Dic[team2Str];

                team2 = _model.Teams.Where(a => a.Name.ToLower() == team2Str.ToLower().Trim()).FirstOrDefault();
                if (team2 == null) throw new Exception("Unknown team");
            }

            var date = Convert.ToDateTime(exQuote.Date);

            var Quote = new Quote
            {
                Date = date,
                Team1 = team1,
                Team2 = team2,
                Q1 = exQuote.Q1.ConvertToDouble(),
                QT = exQuote.QT.ConvertToDouble(),
                Q2 = exQuote.Q2.ConvertToDouble(),
                Game = _model.Games.Where(a => a.HomeTeam == team1 && a.AwayTeam == team2 && a.Date == date).First()
            };

            Quotes.Add(Quote);
        }
    }

    public class TeamNameMapping
    {

        public Dictionary<String, String> Dic { get; set; }

        public TeamNameMapping()
        {
            Dic = new Dictionary<string, string>();
            Dic.Add("Paris SG", "PSG");
            Dic.Add("St Etienne", "St. Etienne");
            Dic.Add("AC Ajaccio", "Ajaccio");
            Dic.Add("Evian TG", "Evian Thonon Gaillard"); 
        }

        private static TeamNameMapping instance;



        public static TeamNameMapping Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TeamNameMapping();
                }
                return instance;
            }
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
}
