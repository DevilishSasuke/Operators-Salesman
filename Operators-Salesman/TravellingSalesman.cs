namespace Operators
{
    public class TravellingSalesman
    {
        public Matrix Distance{ get; private set; }
        public List<int> Path { get; private set; }
        public decimal PathDistance => GetPathDistance();
        private decimal LowerBound { get; set; }
        private Dictionary<int, int> edges { get; set; }
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
            edges = new();

            LowerBound = Distance.LowerBoundValue();
            while(Distance.RowsCount() > 2 && Distance.ColumnsCount() > 2)
            {
                var bound = LowerBound;
                var (node1, node2) = Distance.FindNextEdge(ref bound);
                //(node1, node2) = CalculateOffset(node1, node2);
                edges[node1] = node2;
                LowerBound = bound;
                Distance.SetInfinities(edges);
            }

            return path;
        }

        private (int, int) CalculateOffset(int node1, int node2)
        {
            int rowOffset = 0;
            int columnOffset = 0;

            foreach(var items in edges)
            {
                if (node1 >= items.Key)
                    rowOffset++;
                if (node2 >= items.Value)
                    columnOffset++;
            }

            return (node1 + rowOffset, node2 + columnOffset);
        }

        private decimal GetPathDistance() => 0;
    }
}
