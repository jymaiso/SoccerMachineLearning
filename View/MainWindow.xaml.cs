using DevExpress.Xpf.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestSoc;

namespace View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {




        public MainWindow()
        {
            InitializeComponent();
            this.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Cache.Instance.Go();

            Program.RunSimpleExport();

            //var points38 = Cache.Instance.Teams.Where(a => a.Name == "PSG").First().RankHistory.OrderBy(a => a.Date).Select(a => a).ToList();

            //  seriePoints.Points.AddRange (points38.Select(a=>new SeriesPoint(a.Date, (double)a.Points)));
            //  serieRank.Points.AddRange (points38.Select(a=>new SeriesPoint(a.Date, (double)a.Rank)));


            //  this.Points = 
        }

        public void Displ()
        {
            this.ShowDialog();
        }
    }
}
