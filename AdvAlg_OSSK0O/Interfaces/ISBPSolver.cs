using AdvAlg_OSSK0O.Problems;

namespace AdvAlg_OSSK0O.Interfaces
{
    interface ISBPSolver : ISolver
    {
        SmallestBoundaryPolygon GetProblem { get; }
    }
}
