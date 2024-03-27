using System;
using System.Reflection.Metadata.Ecma335;

namespace Operators
{
    public class TravellingSalesman
    {
        public Matrix Distance{ get; private set; }
        public List<int> Path { get; private set; }
        public decimal PathDistance => GetPathDistance();
        private decimal LowerBound { get; set; }
        public TravellingSalesman(List<List<decimal>> paths)
        {
            int width = paths.Count;
            foreach (var path in paths)
                if (path.Count != width)
                    throw new Exception("Matrix is not square");

            Distance = new Matrix(paths);
        }

        public List<decimal> FindPath()
        {
            List<decimal> path = new(), elements = new();

            LowerBound = Distance.LowerBoundValue();
            Distance.DoSomething();

            return path;
        }

        private decimal GetPathDistance() => 0;
    }
}
