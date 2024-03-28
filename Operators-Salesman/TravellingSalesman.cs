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
            Dictionary<int, int> edges = new();

            LowerBound = Distance.LowerBoundValue();
            while(Distance.RowsCount() > 2 && Distance.ColumnsCount() > 2)
            {
                var bound = LowerBound;
                var (node1, node2) = Distance.FindNextEdge(ref bound);
                edges[node1] = node2;
                LowerBound = bound;
            }

            return path;
        }

        private decimal GetPathDistance() => 0;
    }
}
