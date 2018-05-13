using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvAlg_OSSK0O.Interfaces
{
    interface IGeneration
    {
        List<IChromosome> individuals { get; set; }
        IChromosome GetBestIndividual();
        void AddIndividual(IChromosome individual);
        void AddIndividuals(List<IChromosome> individuals);
    }
}
