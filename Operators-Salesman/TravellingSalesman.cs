namespace Operators
{
    public class TravellingSalesman
    {
        public List<int> Path { get; private set; }         // Путь
        public decimal PathDistance { get; private set; }   // Длина пути
        public Matrix Distance{ get; private set; }         // Матрица для вычислений
        private Dictionary<int, int> Edges { get; set; }    // Прошедшие рёбра

        public TravellingSalesman(List<List<decimal>> paths)
        {
            int len = paths.Count;
            foreach (var path in paths)
                if (path.Count != len)
                    throw new Exception("Matrix is not square");

            Distance = new Matrix(paths);
            Edges = new();
            Path = new();
        }

        public void FindPath()
        {
            // Берём начальное значение нижней границы
            var bound = Distance.LowerBoundValue(); 
            // Пока матрица не достигнет размера 2x2
            while(Distance.RowsCount() > 2 && Distance.ColumnsCount() > 2)
            {
                // Расставляем элементы-бесконечности
                Distance.SetInfinities(Edges);
                // Находим следующий путь
                var (point1, point2) = Distance.FindNextEdge(ref bound);
                Edges[point1] = point2;
            }

            // Добавляем оставшиеся элементы
            foreach(var row in Distance.Rows)
                Edges[row.ActualNumber] = Distance.Columns[1 - row.Infinity].ActualNumber;

            SetNewPath();           // Собираем прошедший путь
            PathDistance = bound;   // Выставляем длину пути
        }

        // Сбор пройденного пути
        private void SetNewPath()
        {
            if (Edges.Count == 0) return;

            Path.Add(Edges.First().Key);
            for (int i = 0; i < Edges.Count; i++)
                Path.Add(Edges[Path.Last()]);
        }
    }
}
