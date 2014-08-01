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

            // set a title


            // set X and Y axis titles
            myPane.XAxis.Title.Text = "X Axis";
            myPane.YAxis.Title.Text = "Y Axis";






            String label = "";
            Color color;

            // ---------------------

            Cache.Instance.Go();

            //  Program.RunSimpleExport();

            var team = Cache.Instance.Teams.Where(a => a.Name == "PSG").First();
            var points38 = team.Games.Select(g => team.RankHistory.Where(r => r.Date == g.Date).First()).OrderBy(a => a.Date).Select(a => a).ToList();


            PointPairList sPoints = GetSerie("Points", Color.Red);
            PointPairList sRank = GetSerie("sRank", Color.Black);

            PointPairList sProbTie = GetSerie("ProbTie", Color.Green);
            PointPairList sProbWin = GetSerie("ProbWin", Color.Orange);
            PointPairList sProbLoose = GetSerie("ProbLoose", Color.Violet);
            PointPairList sProbHomeTie = GetSerie("ProbHomeTie", Color.Gray);
            PointPairList sProbHomeWin = GetSerie("ProbHomeWin", Color.Blue);
            PointPairList sProbHomeLoose = GetSerie("ProbHomeLoose", Color.Brown);
            PointPairList sProbExtTie = GetSerie("ProbExtTie", Color.Pink);
            PointPairList sProbExtWin = GetSerie("ProbExtWin", Color.LightBlue);
            PointPairList sProbExtLoose = GetSerie("ProbExtLoose", Color.LightCoral);

            int index = 0;
            foreach (var item in points38)
            {
                //sPoints.Add(new PointPair(index, (double)item.Points));
                //sRank.Add(new PointPair(index, (double)item.Rank));

                sProbTie.Add(new PointPair(index, (double)item.ProbTie));
                sProbWin.Add(new PointPair(index, (double)item.ProbWin));
                sProbLoose.Add(new PointPair(index, (double)item.ProbLoose));
                sProbHomeTie.Add(new PointPair(index, (double)item.ProbHomeTie));
                sProbHomeWin.Add(new PointPair(index, (double)item.ProbHomeWin));
                sProbHomeLoose.Add(new PointPair(index, (double)item.ProbHomeLoose));
                sProbExtTie.Add(new PointPair(index, (double)item.ProbExtTie));
                sProbExtWin.Add(new PointPair(index, (double)item.ProbExtWin));
                sProbExtLoose.Add(new PointPair(index, (double)item.ProbExtLoose));

                index++;
            }


            // delegate to draw
            zedGraphControl1.AxisChange();
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
