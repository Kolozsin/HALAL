using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvAlg_OSSK0O.Interfaces
{
    class Generation : IGeneration
    {
        public Generation()
        {
            individuals = new List<IChromosome>();
        }
        public List<IChromosome> individuals { get; set; }

        public IChromosome GetBestIndividual()
        {
            return individuals.OrderBy(C => C.Fitness).ToList().First();
        }
        public List<IChromosome> GetElites(int percentage)
        {
            return individuals.OrderBy(C => C.Fitness).Take(individuals.Count * percentage / 100).ToList();
        }
        public void AddIndividual(IChromosome individual)
        {
            this.individuals.Add(individual);
        }

        public void AddIndividuals(List<IChromosome> individuals)
        {
            foreach (var ind in individuals)
            {
                this.individuals.Add(ind);
            }
        }
    }
}
