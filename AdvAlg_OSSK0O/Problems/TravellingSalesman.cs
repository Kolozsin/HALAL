using AdvAlg_OSSK0O.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvAlg_OSSK0O.Problems
{
    class TravellingSalesman : IProblem
    {
        public List<Town> Towns = new List<Town>();
        public class Route : IChromosome
        {
            public List<Town> TownList { get; set; }
            public double Fitness { get; set; }
        }
        public class Town
        {
            public float X { get; set; }
            public float Y { get; set; }
            public int Index { get; set; }
            public override string ToString()
            {
                return Index.ToString();    
            }
            
        }
        public float Objective(List<Town> route)
        {
            float sum_length = 0;
            for (int i = 0; i < route.Count - 1; i++)
			{
                Town t1 = route[i];
		        Town t2 = route[i + 1];
		        sum_length += (float)Math.Sqrt(Math.Pow(t1.X - t2.X, 2) + Math.Pow(t1.Y - t2.Y, 2));
			}
	        return sum_length;
        }
        public void LoadTownsFromFile(string fileName)
        {

        }
        public void saveTownsToFile(string fileName, List<Town> townList)
        {

        }
        public void PrintOutTowns(List<Town> townList)
        {
            for (int i = 0; i < townList.Count; i++)
            {
                Debug.WriteLine("(" + townList[i].X + ", " + townList[i].Y + ")");
            }
        }
    }
}
