using AdvAlg_OSSK0O.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdvAlg_OSSK0O.Problems
{
    class SmallestBoundaryPolygon : IProblem
    {
        static Random r = new Random();
        List<Point> points;
        public List<Point> GetPoints { get { return points; } }

        public SmallestBoundaryPolygon()
        {
            points = new List<Point>();
            float d = DistanceFromLine(new Point(1, 1), new Point(1, 6), new Point(2, 4));
        }
        public class Solution : IChromosome
        {
            public Solution()
            {
                points = new List<Point>();
            }
            public List<Point> points { get; set; }
            public double Fitness { get; set; }
            public static Solution Copy(Solution old)
            {
                Solution newSolution = new Solution();
                foreach (var item in old.points)
                {
                    newSolution.points.Add(new Point() { X = item.X, Y = item.Y });
                }
                newSolution.Fitness = old.Fitness;
                return newSolution;
            }
            public bool SolutionsEquals(Solution b)
            {
                bool eq = true;
                for (int i = 0; i < this.points.Count; i++)
                {
                    if(!this.points[i].Equals(b.points[i]))
                    {
                        eq = false;
                        break;
                    }
                }
                return eq;
            }
        }

        public class Point
        {
            public static Point Create(string[] coord)
            {
                return new Point(int.Parse(coord[0]), int.Parse(coord[1]));
            }
            public Point() { }
            public Point(int x, int y)
            {
                X = x; Y = y;
            }
            public float X { get; set; }
            public float Y { get; set; }
            public float GetDistanceFromPoint(Point p)
            {
                return (float)(Math.Sqrt(Math.Pow(p.X - this.X, 2) + Math.Pow(p.Y - this.Y, 2)));
            }
            public override string ToString()
            {
                return "(" + X + ", " + Y + ")";
            }
            public override bool Equals(object obj)
            {
                Point b = (Point)obj;
                if (this.X == b.X && this.Y == b.Y)
                    return true;
                else
                    return false;
            }
        }
        public void LoadPointsFromFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                int i = 1;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] coord = line.Split('\t');
                    points.Add(new Point() { X = int.Parse(coord[0]), Y = int.Parse(coord[1]) });
                    //Logol(town.Index + ".: (" + town.X + "," + town.Y + ") ");
                    i++;
                }
            }
        }
        public void savePointsToFile(string fileName, List<Point> pointList)
        {
            using (var fileStream = new FileStream(String.Format(fileName),FileMode.OpenOrCreate))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                foreach (Point point in pointList)
                {
                    streamWriter.WriteLine(point);
                }
            }
        }
        public void PrintOutPoints(List<Point> pointList)
        {
            for (int i = 0; i < pointList.Count; i++)
            {
                Debug.WriteLine(pointList[i]);
            }
        }
        #region Helpers
        public float DistanceFromLine(Point lp1, Point lp2, Point p)
        {
            return ((lp2.Y - lp1.Y) * p.X - (lp2.X - lp1.X) * p.Y + lp2.X * lp1.Y - lp2.Y * lp1.X) / 
                (float)(Math.Sqrt(Math.Pow(lp2.Y - lp1.Y, 2) + Math.Pow(lp2.X - lp1.X, 2)));
        }
        public float OuterDistanceToBoundary(List<Point> solution)
        {
           
            float sum_min_distances = 0;
            for (int i = 0; i < points.Count; i++)
            {
                float min_dist = 0;
                for (int j = 0; j < solution.Count; j++)
                {
                    float act_dist = DistanceFromLine(solution[j], solution[(j + 1) % solution.Count], points[i]);
                    if (j == 0 || act_dist < min_dist)
                        min_dist = act_dist;
                }
                if (min_dist < 0)
                    sum_min_distances += -min_dist;
            }
            return (int)sum_min_distances;
        }
        private float LengthOfBoundary(List<Point> solution)
        {
            float sum_length = 0;
            for (int i = 0; i < solution.Count - 1; i++)
            {
                Point p1 = solution[i];
                Point p2 = solution[(i + 1) % solution.Count];
                sum_length += (float)(Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)));
            }
            return sum_length;
        }
        #endregion
        public int Fitness(List<Point> solution)
        {
            return 0;
        }
        
        public float Objective(List<Point> solution)
        {
            return LengthOfBoundary(solution);
        }
        public bool Constraint(List<Point> solution)
        {
            float dist = OuterDistanceToBoundary(solution);
            bool ok = true;
            if (dist == 0)
                for (int i = 0; i < solution.Count - 1; i++)
                {
                    for (int j = i + 1; j < solution.Count; j++)
                    {
                        if (solution[i].Equals(solution[j]))
                        {
                            ok = false;
                            break;
                        }

                    }
                    if (!ok)
                        break;
                }
            else ok = false;
            return dist != 0 || !ok;
        }

        public Solution RandomNeighbor(Solution p, int change_value)
        {
            Solution neighbor = Solution.Copy(p);
            int index = r.Next(0, neighbor.points.Count);
            //int index2 = r.Next(0, neighbor.points.Count);
            do
            {
                neighbor.points[index].X = (float)r.Next((int)(p.points[index].X - change_value), (int)(p.points[index].X + change_value));
                neighbor.points[index].Y = (float)r.Next((int)(p.points[index].Y - change_value), (int)(p.points[index].Y + change_value));
                //neighbor.points[index2].X = (float)r.Next((int)(p.points[index2].X - change_value), (int)(p.points[index2].X + change_value));
                //neighbor.points[index2].Y = (float)r.Next((int)(p.points[index2].Y - change_value), (int)(p.points[index2].Y + change_value));
                
            } while (Constraint(neighbor.points));


            return neighbor;
        }
        public Solution BestNeighbor(Solution p, int change_value)
        {
            Solution neighbor = Solution.Copy(p);
            int minIndex = 0; float minDistance = LengthOfBoundary(neighbor.points);
            float bestX =0,bestY=0;
            for (int i = 0; i < neighbor.points.Count; i++)
            {
                //Console.WriteLine("_________"+i+"_________");
                Point temp = new Point((int)neighbor.points[i].X, (int)neighbor.points[i].Y);
                for (int d = 1; d < change_value; d+=1)
                {
                    float actDistance;
                    //right
                    //Console.WriteLine("right: ");
                    neighbor.points[i].X += d;
                    actDistance = LengthOfBoundary(neighbor.points);
                    if (!Constraint(neighbor.points) && minDistance > actDistance)
                    {
                        bestX = neighbor.points[i].X;
                        bestY = neighbor.points[i].Y;
                        minDistance = actDistance;
                        minIndex = i;
                        //Console.WriteLine("--" + neighbor.points[i]);
                        //Console.WriteLine("-- dist: "+minDistance);
                    }
                    neighbor.points[i] = new Point((int)temp.X, (int)temp.Y);
                    //left
                    //Console.WriteLine("left: ");
                    neighbor.points[i].X -= d;
                    actDistance = LengthOfBoundary(neighbor.points);
                    if (!Constraint(neighbor.points) && minDistance > actDistance)
                    {
                        bestX = neighbor.points[i].X;
                        bestY = neighbor.points[i].Y;
                        minDistance = actDistance;
                        minIndex = i;
                        //Console.WriteLine("--" + neighbor.points[i]);
                        //Console.WriteLine("-- dist: " + minDistance);
                    }
                    neighbor.points[i] = new Point((int)temp.X, (int)temp.Y);
                    //up
                    //Console.WriteLine("up: ");
                    neighbor.points[i].Y += d;
                    actDistance = LengthOfBoundary(neighbor.points);
                    if (!Constraint(neighbor.points) && minDistance > actDistance)
                    {
                        bestX = neighbor.points[i].X;
                        bestY = neighbor.points[i].Y;
                        minDistance = actDistance;
                        minIndex = i;
                        //Console.WriteLine("--" + neighbor.points[i]);
                        //Console.WriteLine("-- dist: " + minDistance);
                    }
                    neighbor.points[i] = new Point((int)temp.X, (int)temp.Y);
                    //down
                    //Console.WriteLine("down: ");
                    neighbor.points[i].Y -= d;
                    actDistance = LengthOfBoundary(neighbor.points);
                    if (!Constraint(neighbor.points) && minDistance > actDistance)
                    {
                        bestX = neighbor.points[i].X;
                        bestY = neighbor.points[i].Y;
                        minDistance = actDistance;
                        minIndex = i;
                        //Console.WriteLine("--" + neighbor.points[i]);
                        //Console.WriteLine("-- dist: " + minDistance);
                    }
                    neighbor.points[i] = new Point((int)temp.X, (int)temp.Y);

                }

            }
            neighbor.points[minIndex].X = bestX;
            neighbor.points[minIndex].Y = bestY;
            //Console.WriteLine(minIndex + ": " + neighbor.points[minIndex]);
            return neighbor;
        }

        internal Solution GenerateFixSolution()
        {
            Solution s = new Solution();
            
            s.points.Add(new Point() { X = 419, Y = 821 });
            s.points.Add(new Point() { X = 622, Y = 390 });
            s.points.Add(new Point() { X = 541, Y = 144 });
            s.points.Add(new Point() { X = 76, Y = -84 });
            s.points.Add(new Point() { X = -134, Y = -18 });
            s.points.Add(new Point() { X = -182, Y = 14 });
            s.points.Add(new Point() { X = -376, Y = 507 });
            s.points.Add(new Point() { X = -136, Y = 851 });
            s.points.Add(new Point() { X = -92, Y =     874 });
            s.points.Add(new Point() { X = 332, Y = 873 });

           
            
            return s;
        }
        internal Solution GenerateRandomSolution()
        {
            Solution s = new Solution();

            float Xmin = points[0].X, Xmax = points[0].X, Ymin = points[0].Y, Ymax = points[0].Y;
            foreach (var point in points)
            {
                if (point.X < Xmin)
                    Xmin = point.X;
                if (point.X > Xmin)
                    Xmax = point.X;
                if (point.Y < Ymin)
                    Ymin = point.Y;
                if (point.Y > Ymax)
                    Ymax = point.Y;
            }

            Point cnter = new Point((int)(Xmin + ((Xmax - Xmin) / 2)), (int)(Ymin + ((Ymax - Ymin) / 2)));
            Point furthestPoint;
            float max = 0;
            foreach (var point in points)
            {
                float dist = cnter.GetDistanceFromPoint(point);
                if(dist > max)
                {
                    max = dist;
                    furthestPoint = point;
                }
            }
            float radius = max + 150;
            do
            {
                s.points.Clear();
                for (int i = 0; i < 10; i++)
                {
                    var noiseX = 40 - r.Next(80);
                    var noiseY = 40 - r.Next(80);
                    var newX = (float)(cnter.X + radius * Math.Cos(i * 36 * 0.0174) + noiseX);
                    var newY = (float)(cnter.Y + radius * Math.Sin(i * 36 * 0.0174) + noiseY);
                    s.points.Add(new Point((int)newX, (int)newY));
                }
            } while (Constraint(s.points));
            

            return s;
        }

        internal float Fitness(Solution p)
        {
            return Objective(p.points);
        }
    }
}
