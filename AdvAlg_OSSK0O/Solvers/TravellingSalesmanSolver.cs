using AdvAlg_OSSK0O.Interfaces;
using AdvAlg_OSSK0O.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace AdvAlg_OSSK0O.Solvers
{
    class TravellingSalesmanSolver : IGeneticAlgorithm
    {
        public event Action<Problems.TravellingSalesman.Route> NewBestRoute;
        static Random r = new Random();
        int generations;
        int MutationChance = 5;
        bool StopCondition { get {  return generations > 40000;} }
        private Problems.TravellingSalesman.Route pbest;

        public Problems.TravellingSalesman.Route Pbest
        {
            get { return pbest; }
            private set { pbest = value;
            NewBestRoute(pbest);
            }
        }
        public void Start(bool random)
        {
            InitialisePopulation(random);
            Evaluation(population.GetCurrentGeneration());
            Pbest = population.GetCurrentGeneration().GetBestIndividual() as Problems.TravellingSalesman.Route;
            generations = 1;
            Logol("\nFitness: " + Pbest.Fitness);
            
            while (!StopCondition)
            {
                Logol("\n" + generations + ".: " + Pbest.Fitness);
                List<IChromosome> MatingPool = null;
                if(generations < 1500)
                     MatingPool = SelectParents(population.GetCurrentGeneration(), 50);
                else if(generations < 3000)
                    MatingPool = SelectParents(population.GetCurrentGeneration(), 30);
                else
                    MatingPool = SelectParents(population.GetCurrentGeneration(), 10);

                //while (MatingPool.Count < population.NumberOfIndividuals)
                //{
                    //List<IChromosome> parents = Selection(MatingPool);
                    population.AddNewGeneration(Crossover(MatingPool));
                //}
                    Mutate();
                generations++;

                Evaluation(population.GetCurrentGeneration());
                Pbest = population.GetCurrentGeneration().GetBestIndividual() as Problems.TravellingSalesman.Route;

                //Thread.Sleep(200);
            }
        }

        public TravellingSalesmanSolver()
        {
            Problem = new TravellingSalesman();
            population = new Population();
            population.NumberOfIndividuals = 100;
            population.NumberOfObjects = 100;
        }
        public void InitialisePopulation(bool random)
        {
            IGeneration g = new Generation();
            if(random)
            {
                Logol("Towns (random):\n");
                //for (int i = 0; i < population.NumberOfObjects; i++)
                //{
                //    (Problem as TravellingSalesman).Towns.Add(new AdvAlg_OSSK0O.Problems.TravellingSalesman.Town() { X = r.Next(20, 400), Y = r.Next(20, 250), Index = (i + 1) });
                //    Logol("(" + (Problem as TravellingSalesman).Towns[i].X + "," + (Problem as TravellingSalesman).Towns[i].Y + ") ");
                //}


                for (int i = 0; i < population.NumberOfObjects / 4; i++)
                {
                    (Problem as TravellingSalesman).Towns.Add(new AdvAlg_OSSK0O.Problems.TravellingSalesman.Town() { X = r.Next(50, 100), Y = r.Next(20, 250), Index = (i + 1) });
                    Logol("(" + (Problem as TravellingSalesman).Towns[i].X + "," + (Problem as TravellingSalesman).Towns[i].Y + ") ");
                }
                for (int i = 0; i < population.NumberOfObjects / 4; i++)
                {
                    (Problem as TravellingSalesman).Towns.Add(new AdvAlg_OSSK0O.Problems.TravellingSalesman.Town() { X = r.Next(350, 400), Y = r.Next(20, 250), Index = (i + 1) });
                    Logol("(" + (Problem as TravellingSalesman).Towns[i].X + "," + (Problem as TravellingSalesman).Towns[i].Y + ") ");
                }
                for (int i = 0; i < population.NumberOfObjects / 4; i++)
                {
                    (Problem as TravellingSalesman).Towns.Add(new AdvAlg_OSSK0O.Problems.TravellingSalesman.Town() { X = r.Next(50, 400), Y = r.Next(50, 100), Index = (i + 1) });
                    Logol("(" + (Problem as TravellingSalesman).Towns[i].X + "," + (Problem as TravellingSalesman).Towns[i].Y + ") ");
                }
                for (int i = 0; i < population.NumberOfObjects / 4; i++)
                {
                    (Problem as TravellingSalesman).Towns.Add(new AdvAlg_OSSK0O.Problems.TravellingSalesman.Town() { X = r.Next(50, 400), Y = r.Next(200, 250), Index = (i + 1) });
                    Logol("(" + (Problem as TravellingSalesman).Towns[i].X + "," + (Problem as TravellingSalesman).Towns[i].Y + ") ");
                }

                //for (int i = 10; i < 400; i+=50)
                //{
                //    for (int j = 10; j < 300; j+=50)
                //    {
                        
                //        (Problem as TravellingSalesman).Towns.Add(new AdvAlg_OSSK0O.Problems.TravellingSalesman.Town() { X = i, Y = j, Index = (i + 1) });
                //        //Logol("(" + (Problem as TravellingSalesman).Towns[i].X + "," + (Problem as TravellingSalesman).Towns[i].Y + ") ");
                //    }
                    
                //}
                population.NumberOfObjects = (Problem as TravellingSalesman).Towns.Count;
                population.NumberOfIndividuals = population.NumberOfObjects * 2;

                for (int i = 0; i < population.NumberOfIndividuals; i++)
                {
                    AdvAlg_OSSK0O.Problems.TravellingSalesman.Route route = new TravellingSalesman.Route() { TownList = (Problem as TravellingSalesman).Towns.ToList() };
                    for (int j = 0; j < population.NumberOfObjects; j++)
                    {
                        int from = r.Next(0, population.NumberOfObjects);
                        int to = r.Next(0, population.NumberOfObjects);

                        AdvAlg_OSSK0O.Problems.TravellingSalesman.Town temp = route.TownList[from];
                        route.TownList[from] = route.TownList[to];
                        route.TownList[to] = temp;
                    }
                    g.individuals.Add(route);
                }
            }
            else
            {
                Logol("Towns (from file):\n");
                using (StreamReader reader = new StreamReader("towns.txt"))
                {
                    int i = 1;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] coord = line.Split('\t');
                        Problems.TravellingSalesman.Town town = new AdvAlg_OSSK0O.Problems.TravellingSalesman.Town() { X = int.Parse(coord[0]), Y = int.Parse(coord[1]), Index = i };
                        (Problem as TravellingSalesman).Towns.Add(town);
                        Logol(town.Index + ".: (" + town.X + "," + town.Y + ") ");
                        i++;
                    }
                }
                int townsNumber = (Problem as TravellingSalesman).Towns.Count;
                population.NumberOfObjects = townsNumber;
                for (int i = 0; i < population.NumberOfIndividuals; i++)
                {
                    AdvAlg_OSSK0O.Problems.TravellingSalesman.Route route = new TravellingSalesman.Route() { TownList = (Problem as TravellingSalesman).Towns.ToList() };
                    for (int j = 0; j < townsNumber; j++)
                    {
                        int from = r.Next(0, townsNumber);
                        int to = r.Next(0, townsNumber);

                        AdvAlg_OSSK0O.Problems.TravellingSalesman.Town temp = route.TownList[from];
                        route.TownList[from] = route.TownList[to];
                        route.TownList[to] = temp;
                    }
                    g.individuals.Add(route);
                }
            }
            
                
            

            
            
            population.AddNewGeneration(g);
        }

        public void Evaluation(IGeneration currentGeneration)
        {
            foreach (Problems.TravellingSalesman.Route route in (currentGeneration as Generation).individuals)
            {
                route.Fitness = (Problem as TravellingSalesman).Objective(route.TownList);
            }
        }
        public List<IChromosome> SelectParents(IGeneration currentGeneration, int percentage)
        {
            return (currentGeneration as Generation).GetElites(percentage);
        }
        public IGeneration Crossover(List<IChromosome> parents)
        {
            Generation newGeneration = new Generation();

            int[] swapPoints = new int[parents.Count / 2];
            for (int i = 0; i < swapPoints.Length; i++)
			{
                int index;
                do
                {
	                index = r.Next(parents.Count);
	            } while (swapPoints.Contains(index));
                swapPoints[i] = index;
			}

            while (newGeneration.individuals.Count < population.NumberOfIndividuals)
            {
                //2 szülő kiválasztása:
                AdvAlg_OSSK0O.Problems.TravellingSalesman.Route parent1, parent2, child;
                parent1 = parents[r.Next(0, parents.Count)] as AdvAlg_OSSK0O.Problems.TravellingSalesman.Route;
                do
                {
                    parent2 = parents[r.Next(0, parents.Count)] as AdvAlg_OSSK0O.Problems.TravellingSalesman.Route;
                } while (parent1 == parent2);

                //leszármazott létrehozása:


                var firstParentGenes = new List<AdvAlg_OSSK0O.Problems.TravellingSalesman.Town>(parent1.TownList);

                child = new TravellingSalesman.Route();
                child.TownList = parent1.TownList.ToList();
                for (int i = 0; i < population.NumberOfObjects; i++)
                {
                    if (swapPoints.Contains(i))
                    {
                        var gene = parent2.TownList[i];
                        firstParentGenes.Remove(gene);
                        firstParentGenes.Insert(i, gene);
                    }
                }
                //child.ReplaceGenes(0, firstParentGenes.ToArray());
                AdvAlg_OSSK0O.Problems.TravellingSalesman.Town[] p1 = firstParentGenes.ToArray();
                for (int i = 0; i < p1.Length; i++)
                {
                    child.TownList[i] = p1[i];
                }

                newGeneration.individuals.Add(child);
            }

            return newGeneration;
        }

        public void Mutate()
        {
            for (int i = 0; i < population.GetCurrentGeneration().individuals.Count; i++)
            {
                if(r.Next(0,100) <= MutationChance)
                {
                    for (int j = 0; j < 1; j++)
                    {
                        int t1 = r.Next(population.NumberOfObjects);
                        int t2 = r.Next(population.NumberOfObjects);
                        //Logol("Mutation: " + t1 + " <=> " + t2);
                        var temp = (population.GetCurrentGeneration().individuals[i] as AdvAlg_OSSK0O.Problems.TravellingSalesman.Route).TownList[t1];
                        (population.GetCurrentGeneration().individuals[i] as AdvAlg_OSSK0O.Problems.TravellingSalesman.Route).TownList[t1] = (population.GetCurrentGeneration().individuals[i] as AdvAlg_OSSK0O.Problems.TravellingSalesman.Route).TownList[t2];
                        (population.GetCurrentGeneration().individuals[i] as AdvAlg_OSSK0O.Problems.TravellingSalesman.Route).TownList[t2] = temp;
                    }
                    
                }
                
            }
        }

        public IProblem Problem { get; set; }

        public IPopulation population { get; set; }
        void Logol(string s)
        {
            try
            {
                using (var fileStream = new FileStream("log.txt", FileMode.Append))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(s);
                }
            }
            catch (Exception)
            {
            }
            
        }

    }
}
