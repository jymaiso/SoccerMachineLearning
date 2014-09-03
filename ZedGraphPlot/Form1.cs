using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TestSoc;
using ZedGraph;

namespace ZedGraphPlot
{
    public partial class Form1 : Form
    {
        // pane used to draw your chart
        GraphPane myPane = new GraphPane();

        // line item
        LineItem myCurveOne;
        LineItem myCurveTwo;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // set your pane
            myPane = zedGraphControl1.GraphPane;

            // set X and Y axis titles
            myPane.XAxis.Title.Text = "X Axis";
            myPane.YAxis.Title.Text = "Y Axis";
            myPane.XAxis.Type = AxisType.Date;

            ModelGames Cache0 = new TestSoc.ModelGames();
            Cache0.ProcessData(new Parameters
            {
                Function = new ConstantFunction(),
                GameCount = 38,
                x0 = 0,
                x1 = 226,
                x2 = 9,
                x3 = 1.3,
                x4 = -0.03,
                k = 0.2
            });

            int index = DisplayCache(Cache0, "Paris SG", "Paris SG", "JPoints", Color.Red);
            index = DisplayCache(Cache0, "Auxerre", "Auxerre", "JPoints", Color.Green);
            index = DisplayCache(Cache0, "Guingamp", "Guingamp", "JPoints", Color.Gray);
            index = DisplayCache(Cache0, "Lorient", "Lorient", "JPoints", Color.Blue);
            index = DisplayCache(Cache0, "Sochaux", "Sochaux", "JPoints", Color.Purple);

            //ModelGames model = new ModelGames();
            //ModelQuotes qs = new ModelQuotes(model);
            //PointPairList sV0 = GetSerie("x1", Color.Red);

            //for (int i = 0; i < 10; i++)
            //{
            //    double val =  i *30;

            //    model.ProcessData(new Parameters
            //    {
            //        Function = new LinearFunction(2),
            //        GameCount = 38,
            //        x0 = 0,
            //        x1 = 226,
            //        x2 = 9,
            //        x3 = 1.3,
            //        x4 = -0.03,
            //        k = val
            //    });

            //    qs.LoadData();


            //    sV0.Add(new PointPair(val, Math.Sqrt(qs.Quotes.Sum(a => a.MyMSE) / qs.Quotes.Count)));
            //}



            zedGraphControl1.AxisChange();

        }


        private void DisplayProba()
        {
            // ---------------------

            ModelGames Cache0 = new TestSoc.ModelGames();
            Cache0.ProcessData(new Parameters
            {
                Function = new ConstantFunction(),
                GameCount = 74,
            });

            int index = DisplayCache(Cache0, "ConstantFunction - true", "PSG", "ProbWin", Color.Red);

            ModelGames Cache1 = new TestSoc.ModelGames();
            Cache1.ProcessData(new Parameters
            {
                Function = new LinearFunction(2),
                GameCount = 74,
            });

            DisplayCache(Cache1, "ConstantFunction - false", "PSG", "ProbWin", Color.Blue);

            ModelGames Cache2 = new TestSoc.ModelGames();
            Cache2.ProcessData(new Parameters
            {
                Function = new PowFunction(),
                GameCount = 74,
            });

            DisplayCache(Cache2, "ExpFunction - true", "PSG", "ProbWin", Color.Green);

            ModelGames Cache3 = new TestSoc.ModelGames();
            Cache3.ProcessData(new Parameters
            {
                Function = new LogFunction(),
                GameCount = 74,
            });

            DisplayCache(Cache3, "ExpFunction - false", "PSG", "ProbWin", Color.Black);
        }

        private int DisplayCache(ModelGames Cache0, String label, String team, String property, Color color)
        {
            var date = new DateTime(2000, 06, 01);
            var team0 = Cache0.Teams.Where(a => a.Name == team).First();
            var points0 = team0.Games.Where(a => a.Date > date).Select(g => team0.StatsHistory.Where(r => r.Date == g.Date).First()).OrderBy(a => a.Date).Select(a => a).ToList();
            var pi = typeof(TeamStats).GetProperty(property);
            PointPairList sV0 = GetSerie(label, color);
            int index = 0;
            foreach (var stat in points0)
            {
                sV0.Add(new PointPair(ConvertDateToXdate(stat.Date), (double)pi.GetValue(stat)));
                index++;
            }
            return index;
        }

        private PointPairList GetSerie(String label, Color color)
        {

            PointPairList listPointsOne = new PointPairList();
            myCurveOne = myPane.AddCurve(label, listPointsOne, color, SymbolType.None);
            myCurveOne.Line.Width = 2;
            return listPointsOne;
        }

        public XDate ConvertDateToXdate(DateTime date)
        {
            return new XDate(date.ToOADate());
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Application.DoEvents();
        }
    }


}
