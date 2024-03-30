using System.Runtime.CompilerServices;

namespace Operators
{
    public class TravellingSalesman
    {
        public Matrix Distance{ get; private set; }
        public List<int> Path { get; private set; }
        public decimal PathDistance => LowerBound;
        private decimal LowerBound { get; set; }
        private Dictionary<int, int> edges { get; set; }
        public TravellingSalesman(List<List<decimal>> paths)
        {
            int len = paths.Count;
            foreach (var path in paths)
                if (path.Count != len)
                    throw new Exception("Matrix is not square");

            Distance = new Matrix(paths);
        }

        public void FindPath()
        {
            Path = new();
            edges = new();

            LowerBound = Distance.LowerBoundValue();
            while(Distance.RowsCount() > 2 && Distance.ColumnsCount() > 2)
            {
                var bound = LowerBound;
                var (node1, node2) = Distance.FindNextEdge(ref bound);
                edges[node1] = node2;
                LowerBound = bound;
                Distance.SetInfinities(edges);
            }

            for (int i = 0; i < Distance.RowsCount(); i++)
                edges[Distance.Rows[i].ActualNumber] = Distance.Columns[1 - i].ActualNumber;

            Path.Add(edges.First().Key);
            for(int i = 0; i < edges.Count; i++)
                Path.Add(edges[Path.Last()]);
        }
    }
}
