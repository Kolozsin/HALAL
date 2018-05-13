using AdvAlg_OSSK0O.Interfaces;
using AdvAlg_OSSK0O.Problems;
using System;
using System.IO;

namespace AdvAlg_OSSK0O.Solvers
{
    class SimulatedAnnealing : ISBPSolver
    {
        #region Variables/Properties
        public event Action<SmallestBoundaryPolygon.Solution, SmallestBoundaryPolygon.Solution> NewSolution;
        SmallestBoundaryPolygon problem;
        bool stop = false;

        const double Boltzmann = 1.0e-16;//1.3807e-16
        int t, Epsilon = 50;
        double Tmax = 20000;
        SmallestBoundaryPolygon.Solution p_opt, p;

        
        internal SmallestBoundaryPolygon.Solution P_opt
        {
            get { return p_opt; }
            set
            {
                p_opt = value;
            }
        }
        internal SmallestBoundaryPolygon.Solution P
        {
            get { return p; }
            set
            {
                p = value;
            }
        }
        #endregion
        static Random r = new Random();
        //Constructor
        public SimulatedAnnealing()
        {
            problem = new SmallestBoundaryPolygon();
            problem.LoadPointsFromFile("SBP_Points.txt");
        }
        
        private float Temperature()
        {
            return (float)(Tmax * (1 - t / Tmax));
        }
        public bool StoppingCondition()
        {
            return Temperature() <= 0 && !stop;
        }
        public float AcceptanceProbability(float deltaE, float T)
        {
            return (float)Math.Exp(-deltaE / (Boltzmann * T));
        }
        
        void Optimize()
        {
            stop = false;
            P = problem.GenerateFixSolution();
            P.Fitness = problem.Fitness(P);
            

            P_opt = SmallestBoundaryPolygon.Solution.Copy(P);

            t = 0;
            while (!StoppingCondition())
            {
                //Thread.Sleep(5);
                //get random neighbor
                SmallestBoundaryPolygon.Solution q = problem.RandomNeighbor(P, Epsilon);
                q.Fitness = problem.Fitness(q);

                float deltaE = (float)q.Fitness - (float)P.Fitness;

                if (deltaE < 0)
                {
                    P = SmallestBoundaryPolygon.Solution.Copy(q);

                    if (P.Fitness < P_opt.Fitness)
                    {
                        //New Optimal
                        P_opt = SmallestBoundaryPolygon.Solution.Copy(P);
                        Logol("--------\n Iteration: " + t + ", Temperature: " + Temperature().ToString());
                        Logol("BEST: " + P_opt.Fitness, P_opt);
                    }
                }
                else
                {
                    float T = Temperature();
                    double rand = r.NextDouble();
                    //Logol("random: " + rand + ", P: " + AcceptanceProbability(deltaE, T).ToString("0.0000"));
                    if (rand < AcceptanceProbability(deltaE, T))
                    {
                        P = SmallestBoundaryPolygon.Solution.Copy(q);
                        Logol("random OK", q);
                    }
                }
                NewSolution(P_opt,q);
                t++;
            }
        }

       

        #region Log
        void Logol(string s)
        {
            try
            {
                using (var fileStream = new FileStream("log_SMP_SA.txt", FileMode.Append))
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
                using (var fileStream = new FileStream("log_SMP_SA.txt", FileMode.Append))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(ss + ":");
                    foreach (var point in s.points)
                    {
                        streamWriter.WriteLine("("+point.X+"\t"+point.Y+")");
                    }
                    
                }
            }
            catch (Exception)
            {
            }

        }
        #endregion

        

        public SmallestBoundaryPolygon GetProblem
        {
            get { return problem; }
        }

        public void Start()
        {
            P = null;
            P_opt = null;
            Optimize();
        }

        public void Stop()
        {
            stop = false;
        }
    }
}
