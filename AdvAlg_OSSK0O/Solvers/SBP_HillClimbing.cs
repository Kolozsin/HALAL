using AdvAlg_OSSK0O.Interfaces;
using AdvAlg_OSSK0O.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvAlg_OSSK0O.Solvers
{
    class SBP_HillClimbing : ISBPSolver
    {
        #region Variables/Properties
        public event Action<SmallestBoundaryPolygon.Solution, SmallestBoundaryPolygon.Solution> NewSolution;
        SmallestBoundaryPolygon problem;
        bool stop = false;

        
        int t, Epsilon = 10;
        SmallestBoundaryPolygon.Solution p;

        public SmallestBoundaryPolygon GetProblem
        { get { return problem; } }
        
        public SmallestBoundaryPolygon.Solution P
        {
            get { return p; }
            set { p = value; }
        }
        SmallestBoundaryPolygon.Solution P_opt;
        #endregion
        static Random r = new Random();
        public enum SolutionType { Stochastic, SteepestAscent, RandomRestart}
        SolutionType solutionType;
        //Constructor
        public SBP_HillClimbing(SolutionType solutionType)
        {
            this.solutionType = solutionType;
            problem = new SmallestBoundaryPolygon();
            problem.LoadPointsFromFile("SBP_Points.txt");
        }
        
        
        public bool StoppingCondition()
        {
            return t > 10000 || stop;
        }
        

        
        void Optimize_Stochastic()
        {
            
            P = problem.GenerateFixSolution();
            P.Fitness = problem.Fitness(P);

            t = 0;
            while (!StoppingCondition())
            {
                SmallestBoundaryPolygon.Solution q = problem.RandomNeighbor(P, Epsilon);
                q.Fitness = problem.Fitness(q);

                float deltaE = (float)q.Fitness - (float)P.Fitness;

                if (deltaE < 0)
                {
                    P = SmallestBoundaryPolygon.Solution.Copy(q);
                    
                    Logol(t + ": BEST: " + p.Fitness, p);
                }
                NewSolution(P, q);
                t++;
            }
        }
        void Optimize_SteepestAscent()
        {
            bool stuck = false;
            P = problem.GenerateFixSolution();
            P.Fitness = problem.Fitness(P);

            t = 0;
            while (!stuck && !StoppingCondition())
            {
                SmallestBoundaryPolygon.Solution q = problem.BestNeighbor(P, Epsilon);
                q.Fitness = problem.Fitness(q);

                float deltaE = (float)q.Fitness - (float)P.Fitness;

                if (deltaE < 2)
                {
                    P = SmallestBoundaryPolygon.Solution.Copy(q);

                    Logol(t + ": BEST: " + p.Fitness, p);
                }
                else
                {
                    stuck = true;
                }
                NewSolution(P, null);
                t++;
            }
        }
        void Optimize_RandomRestart()
        {
            List<SmallestBoundaryPolygon.Solution> V = new List<SmallestBoundaryPolygon.Solution>();
            

            t = 0;
            while (!StoppingCondition())
            {
                bool ok;
                do
	            {
                    ok = true;
                    P = problem.GenerateRandomSolution();
                    
	                foreach (var sol in V)
                    {
                        if(P.SolutionsEquals(sol))
                        {
                            ok = false;
                            break;
                        }
                    }
	            } while (!ok);

                P.Fitness = problem.Fitness(P);
                if (P_opt == null)
                    P_opt = SmallestBoundaryPolygon.Solution.Copy(P);
                bool stuck = false;

                while (!stuck && !StoppingCondition())
                {
                    SmallestBoundaryPolygon.Solution q = problem.BestNeighbor(P, Epsilon);
                    q.Fitness = problem.Fitness(q);
                    float deltaE = (float)q.Fitness - (float)P.Fitness;

                    if (deltaE < 0)
                    {
                        P = SmallestBoundaryPolygon.Solution.Copy(q);
                        V.Add(q);
                        if (P_opt.Fitness > P.Fitness)
                            P_opt = SmallestBoundaryPolygon.Solution.Copy(P);
                        Logol(t + ": BEST: " + p.Fitness, p);
                    }
                    else
                    {
                        stuck = true;
                    }
                    NewSolution(P_opt, P);
                }

                

                
                
                t++;
            }
        }

        public void Start()
        {
            P = null;
            stop = false;
            switch (solutionType)
            {
                case SolutionType.Stochastic:
                    Optimize_Stochastic();
                    break;
                case SolutionType.SteepestAscent:
                    Optimize_SteepestAscent();
                    break;
                case SolutionType.RandomRestart:
                    Optimize_RandomRestart();
                    break;
                default:
                    break;
            }
        }
        public void Stop()
        {
            stop = true;
        }

        #region Log
        void Logol(string s)
        {
            try
            {
                using (var fileStream = new FileStream("log_SBP_HillClimbing.txt", FileMode.Append))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(s);
                }
            }
            catch (Exception)
            {
            }

        }
        void Logol(string ss, SmallestBoundaryPolygon.Solution s)
        {
            try
            {
                using (var fileStream = new FileStream("log_SBP_HillClimbing.txt", FileMode.Append))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(ss + ":");
                    foreach (var point in s.points)
                    {
                        streamWriter.WriteLine(point.X + "\t" + point.Y);
                    }
                    
                }
            }
            catch (Exception)
            {
            }

        }
        #endregion



    }
}
