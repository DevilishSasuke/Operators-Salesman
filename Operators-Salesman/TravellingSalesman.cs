using System;

namespace Operators
{
    public class TravellingSalesman
    {
        public Matrix Distance{ get; private set; }
        public TravellingSalesman(List<List<decimal>> paths)
        {
            int width = paths.Count;
            foreach (var path in paths)
                if (path.Count != width)
                    throw new Exception("Matrix is not square");

            Distance = new Matrix(paths);
        }
    }
}
