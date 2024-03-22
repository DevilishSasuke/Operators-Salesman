using System;

namespace Operators
{
    public class TravellingSalesman
    {
        public decimal[][] Paths { get; private set; }
        public TravellingSalesman(decimal[][] paths)
        {
            int width = paths.Length;
            foreach (var path in paths)
                if (path.Length != width)
                    throw new Exception("Matrix is not square");

            Paths = paths;
        }
    }
}
