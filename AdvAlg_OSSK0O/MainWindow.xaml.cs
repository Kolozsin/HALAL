using AdvAlg_OSSK0O.Solvers;
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

namespace AdvAlg_OSSK0O
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TravellingSalesmanSolver TSM;
        public MainWindow()
        {
            InitializeComponent();
            TSM = new TravellingSalesmanSolver();
            TSM.NewBestRoute += DrawingRoute;
        }

        private void DrawingRoute(Problems.TravellingSalesman.Route route)
        {
            Console.WriteLine("RAJZ");
            Dispatcher.BeginInvoke((Action)(() => {
                distancelabel.Content = (int)route.Fitness;
                Map.Children.Clear();
                Problems.TravellingSalesman.Town temp = null;
                foreach (Problems.TravellingSalesman.Town town in route.TownList)
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Stroke = Brushes.Red; ellipse.Width = 10; ellipse.Height = 10;
                    ellipse.StrokeThickness = 10;
                    Canvas.SetTop(ellipse, (town.Y * 2) - 5);
                    Canvas.SetLeft(ellipse, (town.X * 2) - 5);
                    Map.Children.Add(ellipse);

                    if (temp != null)
                    {
                        Line r = new Line();
                        r.X1 = temp.X * 2; r.Y1 = temp.Y * 2;
                        r.X2 = town.X * 2; r.Y2 = town.Y * 2;
                        r.Stroke = Brushes.Yellow;
                        r.StrokeThickness = 1;
                        Map.Children.Add(r);
                    }
                    temp = town;

                    
                    
                }
            
            }));
            
        }

        private void StartRandomClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                TSM.Start(true);
            });
            
        }

        private void StartFromFileClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                TSM.Start(false);
            });
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
