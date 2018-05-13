using AdvAlg_OSSK0O.Interfaces;
using AdvAlg_OSSK0O.Solvers;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdvAlg_OSSK0O
{
    /// <summary>
    /// Interaction logic for SmallestBoundaryPolygonWindow.xaml
    /// </summary>
    public partial class SmallestBoundaryPolygonWindow : Window
    {
        ISBPSolver solver;
        const int PlusPixels = 500;
        public SmallestBoundaryPolygonWindow()
        {
            InitializeComponent();
            //solver = new SimulatedAnnealing();
            solver = new SimulatedAnnealing();
            //solver = new SBP_HillClimbing();
            //solver.NewSolution += DrawingSolution;
        }

        private void DrawingSolution(Problems.SmallestBoundaryPolygon.Solution opt, Problems.SmallestBoundaryPolygon.Solution sol)
        {

            Dispatcher.BeginInvoke((Action)(() =>
            {
                Map.Children.Clear();
                DrawingPoints();
                if(sol != null)
                for (int j = 0; j < sol.points.Count; j++)
                {
                    Line line = new Line();

                    line.X1 = sol.points[j].Y; 
                    line.Y1 = sol.points[j].X;

                    line.X2 = sol.points[(j + 1) % sol.points.Count].Y;
                    line.Y2 = sol.points[(j + 1) % sol.points.Count].X;

                    line.Stroke = Brushes.Black;
                    line.StrokeThickness = 1;
                    Map.Children.Add(line);

                }
                for (int i = 0; i < opt.points.Count; i++)
                {
                    Line best = new Line();

                    best.X1 = opt.points[i].Y; 
                    best.Y1 = opt.points[i].X;

                    best.X2 = opt.points[(i + 1) % opt.points.Count].Y;
                    best.Y2 = opt.points[(i + 1) % opt.points.Count].X;

                    best.Stroke = Brushes.Blue;
                    best.StrokeThickness = 1;
                    Map.Children.Add(best);

                    Ellipse ellipse = new Ellipse();
                    ellipse.Stroke = Brushes.Magenta; ellipse.Width = 7; ellipse.Height = 7;
                    ellipse.StrokeThickness = 10;
                    Canvas.SetTop(ellipse, best.Y1);
                    Canvas.SetLeft(ellipse, best.X1);
                    Map.Children.Add(ellipse);

                }
                Label l = new Label();
                l.Content = opt.Fitness;
                Canvas.SetTop(l, 20);
                Canvas.SetLeft(l, 20);
                Map.Children.Add(l);
                
            }));
        }
        private void DrawingPoints()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {

                foreach (Problems.SmallestBoundaryPolygon.Point point in solver.GetProblem.GetPoints)
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Stroke = Brushes.Red; ellipse.Width = 7; ellipse.Height = 7;
                    ellipse.StrokeThickness = 10;
                    Canvas.SetTop(ellipse, point.X);
                    Canvas.SetLeft(ellipse, point.Y);
                    Map.Children.Add(ellipse);
                }

            }));
        }

        private void ChooseSolver()
        {
            string solv = ((ComboBoxItem)combobox.SelectedItem).Content.ToString();
            switch (solv)
            {
                case "Simulated Annealing":
                    solver = new SimulatedAnnealing();
                    (solver as SimulatedAnnealing).NewSolution += DrawingSolution;
                    break;
                case "Hill Climbing Stochastic":
                    solver = new SBP_HillClimbing(SBP_HillClimbing.SolutionType.Stochastic);
                    (solver as SBP_HillClimbing).NewSolution += DrawingSolution;
                    break;
                case "Hill Climbing Steepest Ascent":
                    solver = new SBP_HillClimbing(SBP_HillClimbing.SolutionType.SteepestAscent);
                    (solver as SBP_HillClimbing).NewSolution += DrawingSolution;
                    break;
                case "Hill Climbing Random Restart":
                    solver = new SBP_HillClimbing(SBP_HillClimbing.SolutionType.RandomRestart);
                    (solver as SBP_HillClimbing).NewSolution += DrawingSolution;
                    break;
                default:
                    break;
            }
        }
        private void StartRandomClick(object sender, RoutedEventArgs e)
        {
            ChooseSolver();
            Task.Run(() =>
            {
                solver.Start();
            });

        }

        private void StartFromFileClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                //solver.Start();
            });
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            solver.Stop();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DrawingPoints();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SmallestBoundaryPolygonWindow Hack = new SmallestBoundaryPolygonWindow();
            Hack.Show();
            this.Close();
        }
    }
}
