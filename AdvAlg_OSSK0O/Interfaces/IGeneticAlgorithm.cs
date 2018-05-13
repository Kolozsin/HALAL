using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvAlg_OSSK0O.Interfaces
{
    interface IGeneticAlgorithm
    {
        IProblem Problem { get; set; }
        IPopulation population { get; set; }
        void InitialisePopulation(bool random);
        void Evaluation(IGeneration population);

        List<IChromosome> SelectParents(IGeneration currentGeneration, int percentage);
        //List<IChromosome> Selection(IGeneration matingPool);
        IGeneration Crossover(List<IChromosome> parents);
        void Mutate();
    }
}
