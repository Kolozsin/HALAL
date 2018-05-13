using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvAlg_OSSK0O.Interfaces
{
    class Population : IPopulation
    {
        public Population()
        {
            generations = new List<IGeneration>();
        }
        public List<IGeneration> generations { get; set; }

        public void AddNewGeneration(IGeneration newGeneration)
        {
            generations.Add(newGeneration);
        }

        public IGeneration GetCurrentGeneration()
        {
            return generations.Last();
        }
        public int NumberOfObjects { get; set; }
        public int MaxNumberOfGenerations { get; set; }


        public int NumberOfIndividuals { get; set; }
    }
}
