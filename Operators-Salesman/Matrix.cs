namespace Operators
{
    public class Matrix
    {
        private List<MatrixUnit> Columns { get; set; }
        private List<MatrixUnit> Rows { get; set; }

        public Matrix(List<List<decimal>> distance)
        {
            Rows = new List<MatrixUnit>();
            Columns = new List<MatrixUnit>();
            var isFirst = true;
            for (int i = 0; i < distance.Count; i++)
            {
                Rows.Add(new MatrixUnit(distance[i], i));
                for (int j = 0; j < distance.Count; j++)
                {
                    if (isFirst) Columns.Add(new MatrixUnit(j));
                    Columns[j].Add(distance[i][j]);
                }
                isFirst = false;
            }
        }

        public decimal this[int i, int j]
        {
            get
            {
                if (Rows[i].IsEmpty || Columns[i].IsEmpty)
                    throw new Exception("Is empty");
                else return Rows[i][j];
            }
            set => Rows[i][j] = value;
        }

        public List<decimal> GetColumn(int columnNumber) => Columns[columnNumber];
        public List<decimal> GetRow(int rowNumber) => Rows[rowNumber];
    }
}
