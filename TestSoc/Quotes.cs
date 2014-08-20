using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel;

namespace TestSoc
{
    public class QuotesModel
    {
        public Model _model { get; set; }

        public List<Quote> Quotes { get; set; }

        public QuotesModel(Model model)
        {
            this._model = model;
            Quotes = new List<Quote>();
        }

        public void LoadData()
        {
            string file = Path.Combine(Environment.CurrentDirectory, "quote.xlsx");

            this.AddQuotes(file, 2013);


        }

        private void AddQuotes(string file, int year)
        {
            var excel = new ExcelQueryFactory(file);
            var qs = from c in excel.Worksheet<ExcelQuote>(String.Format("{0}_{1}", year, year + 1))
                     select c;


            foreach (var q in qs)
            {
                AddQuote(q, year);
            }
        }

        private void AddQuote(ExcelQuote exQuote, int year)
        {

            String team1Str = exQuote.Teams.Substring(0, exQuote.Teams.IndexOf("-") - 1).Trim();
            String team2Str = exQuote.Teams.Substring(exQuote.Teams.IndexOf("-") + 1, exQuote.Teams.Length - exQuote.Teams.IndexOf("-") - 1).Trim();

            Team team1 = _model.Teams.Where(a => a.Name.ToLower() == team1Str.ToLower().Trim()).FirstOrDefault();
            if (team1 == null)
            {
                team1Str = TeamNameMapping.Instance.Dic[team1Str];

                team1 = _model.Teams.Where(a => a.Name.ToLower() == team1Str.ToLower().Trim()).FirstOrDefault();
                if (team1 == null)
                {
                    throw new Exception("Unknown team");
                }
            }


            Team team2 = _model.Teams.Where(a => a.Name.ToLower() == team2Str.ToLower().Trim()).FirstOrDefault();
            if (team2 == null)
            {
                team2Str = TeamNameMapping.Instance.Dic[team2Str];

                team2 = _model.Teams.Where(a => a.Name.ToLower() == team2Str.ToLower().Trim()).FirstOrDefault();
                if (team2 == null)
                {
                    throw new Exception("Unknown team");
                }
            }

            var date = Convert.ToDateTime(exQuote.Date);

            if (date == DateTime.MinValue)
            {
                Console.WriteLine(); 
            }

            var Quote = new Quote
            {
                Date = date,
                Team1 = team1,
                Team2 = team2,
                Q1 = exQuote.Q1.ConvertToDouble(),
                QT = exQuote.QT.ConvertToDouble(),
                Q2 = exQuote.Q2.ConvertToDouble(),
                Game = _model.Games.Where(a => a.Team1 == team1 && a.Team2 == team2 && a.Date > date.AddDays(-10) && a.Date < date.AddDays(10)).First()
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
}
