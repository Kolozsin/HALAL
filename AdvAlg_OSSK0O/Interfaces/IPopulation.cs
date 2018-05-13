using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvAlg_OSSK0O.Interfaces
{
    interface IPopulation
    {
        List<IGeneration> generations { get; set; }
        void AddNewGeneration(IGeneration newGeneration);
        IGeneration GetCurrentGeneration();
        int MaxNumberOfGenerations { get; set; }

        int NumberOfIndividuals { get; set; }
        int NumberOfObjects { get; set; }

    }
}
