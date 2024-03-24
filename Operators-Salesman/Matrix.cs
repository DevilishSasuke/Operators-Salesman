namespace Operators
{
    public class Matrix
    {
        private List<Column> Columns { get; set; }
        private List<Row> Rows { get; set; }

        public Matrix(List<List<decimal>> distance)
        {
            Rows = new List<Row>();
            Columns = new List<Column>();
            for (int i = 0; i < distance.Count; i++)
            {
                Rows.Add(new Row(i, distance));
                Columns.Add(new Column(i, distance));
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
