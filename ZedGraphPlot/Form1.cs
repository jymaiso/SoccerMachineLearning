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

            // DisplayProba();

            Cache Cache2 = new TestSoc.Cache();
            Cache2.Go(new Parameters
            {
                Function = new ConstantFunction(),
                GameCount = 74,
                EnableStrongWeakOpposite = false
            });
            DisplayProbaCorrelation(Cache2, "Const-74-false", "ProbExtWin", (a) => a.ProbExtWin, Color.Green);
            //DisplayProbaCorrelation(Cache1, "ProbLoose", "ProbLoose", (a) => a.ProbLoose, Color.Blue);
            //DisplayProbaCorrelation(Cache1, "ProbHomeWin", "ProbHomeWin", (a) => a.ProbHomeWin, Color.Pink);
            //DisplayProbaCorrelation(Cache1, "ProbHomeLoose", "ProbHomeLoose", (a) => a.ProbHomeLoose, Color.Red);
            //DisplayProbaCorrelation(Cache1, "ProbExtWin", "ProbExtWin", (a) => a.ProbExtWin, Color.Black);
            //DisplayProbaCorrelation(Cache1, "ProbExtLoose", "ProbExtLoose", (a) => a.ProbExtLoose, Color.Gray);

            Cache Cache1 = new TestSoc.Cache();
            Cache1.Go(new Parameters
            {
                Function = new ConstantFunction(),
                GameCount = 74,
                EnableStrongWeakOpposite = true
            });
            DisplayProbaCorrelation(Cache1, "Const-74-true", "ProbExtWin", (a) => a.ProbExtWin, Color.Blue);

            //Cache Cache0 = new TestSoc.Cache();
            //Cache0.Go(new Parameters
            //{
            //    Function = new ExpFunction(),
            //    GameCount = 100,
            //    EnableStrongWeakOpposite = true
            //});
            //DisplayProbaCorrelation(Cache0, "Exp - 100 - true", "ProbExtWin", (a) => a.ProbExtWin, Color.Red);

            zedGraphControl1.AxisChange();
        }

        private void DisplayProbaCorrelation(Cache Cache0, String label, String property, Func<TeamStats, double> propFunc, Color color)
        {
            PointPairList sV0 = GetSerie(label, color);
            int index = 0;

            var date = new DateTime(2000, 06, 01);
            double intervalCount = 20;
            var stats = Cache0.Games.Where(a => a.Date > date).Select(a => a.Stat1).ToList();
            stats.AddRange(Cache0.Games.Where(a => a.Date > date).Select(a => a.Stat2).ToList());
            var pi = typeof(TeamStats).GetProperty(property);

            double min = stats.Where(a => !double.IsNaN((double)pi.GetValue(a))).Min(propFunc);
            double max = stats.Where(a => !double.IsNaN((double)pi.GetValue(a))).Max(propFunc);

            //double min = 0;
            //double max = 1;

            double interval = (max - min) / intervalCount;

            for (int i = 0; i < intervalCount; i++)
            {
                var localMin = (min + i * interval);
                var localMax = (min + (i + 1) * interval);

                var localStats = stats.Where(a => !double.IsNaN((double)pi.GetValue(a)) && localMin <= (double)pi.GetValue(a) && (double)pi.GetValue(a) < localMax);
                var avg = localStats.Average(a => (double?)(a.Team == a.Game.Winner ? 1 : 0));


                sV0.Add(new PointPair((localMin + localMax) / 2, avg ?? 0, localStats.Count().ToString()));

            }
            Console.WriteLine();
        }


        private void DisplayProba()
        {
            // ---------------------

            Cache Cache0 = new TestSoc.Cache();
            Cache0.Go(new Parameters
            {
                Function = new ConstantFunction(),
                GameCount = 74,
                EnableStrongWeakOpposite = true
            });

            int index = DisplayCache(Cache0, "ConstantFunction - true", "PSG", "ProbWin", Color.Red);

            Cache Cache1 = new TestSoc.Cache();
            Cache1.Go(new Parameters
            {
                Function = new LinearFunction(2),
                GameCount = 74,
                EnableStrongWeakOpposite = true
            });

            DisplayCache(Cache1, "ConstantFunction - false", "PSG", "ProbWin", Color.Blue);

            Cache Cache2 = new TestSoc.Cache();
            Cache2.Go(new Parameters
            {
                Function = new ExpFunction(),
                GameCount = 74,
                EnableStrongWeakOpposite = true
            });

            DisplayCache(Cache2, "ExpFunction - true", "PSG", "ProbWin", Color.Green);

            Cache Cache3 = new TestSoc.Cache();
            Cache3.Go(new Parameters
            {
                Function = new LogFunction(),
                GameCount = 74,
                EnableStrongWeakOpposite = true
            });

            DisplayCache(Cache3, "ExpFunction - false", "PSG", "ProbWin", Color.Black);
        }

        private int DisplayCache(Cache Cache0, String label, String team, String property, Color color)
        {
            var date = new DateTime(2000, 06, 01);
            var team0 = Cache0.Teams.Where(a => a.Name == team).First();
            var points0 = team0.Games.Where(a => a.Date > date).Select(g => team0.StatsHistory.Where(r => r.Date == g.Date).First()).OrderBy(a => a.Date).Select(a => a).ToList();
            var pi = typeof(TeamStats).GetProperty(property);
            PointPairList sV0 = GetSerie(label, color);
            int index = 0;
            foreach (var stat in points0)
            {
                sV0.Add(new PointPair(index, (double)pi.GetValue(stat)));
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
    }

}
