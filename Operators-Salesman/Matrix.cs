using System.Xml.Linq;

namespace Operators
{
    public class Matrix
    {
        private List<Column> Columns { get; set; }
        private List<Row> Rows { get; set; }
        public List<List<decimal>> Distance { get; private set; }

        public Matrix(List<List<decimal>> distance)
        {
            Rows = new List<Row>();
            Columns = new List<Column>();
            Distance = distance;
            for (int i = 0; i < Distance.Count; i++)
            {
                Rows.Add(new Row(i, Distance));
                Columns.Add(new Column(i, Distance));
            }
        }

        public decimal LowerBoundValue()
        {
            var rowElements = MinRowElements();
            SubInRows(rowElements);
            var columnElements = MinColumnElements();
            SubInColumns(columnElements);

            return rowElements.Sum() + columnElements.Sum();
        }

        public void DoSomething()
        {
            var rowElements = MinRowWithZeroReplacing();
            var columnElements = MinColumnWithZeroReplacing();

            var (indexI, indexJ) = MaxIntersection(rowElements, columnElements);
            ExcludeEdge(indexI, indexJ);
        }

        public (int, int) MaxIntersection(List<decimal> rows, List<decimal> columns)
        {
            decimal maxValue = decimal.MinValue, value;
            int resI = 0, resJ = 0;

            for (int i = 0; i < Distance.Count; i++)
                for (int j = 0; j < Distance[0].Count; j++)
                {
                    if (Distance[i][j] == 0 && i != j)
                    {
                        value = rows[i] + columns[j];
                        if (value > maxValue)
                        {
                            maxValue = value;
                            (resI, resJ) = (i, j);
                        }
                    }

                }

            return (resI, resJ);
        } 

        public void ExcludeEdge(int i, int j)
        {

        }

        public void SubInRows(List<decimal> values)
        {
            for (int i = 0; i < Rows.Count; i++)
                for(int j = 0; j < Columns.Count; j++)
                {
                    if (j == Rows[i].Number) continue;
                    Rows[i][j] -= values[i];
                }
        }
        
        public void SubInColumns(List<decimal> values)
        {
            for (int i = 0; i < Columns.Count; i++)
                for (int j = 0; j < Rows.Count; j++)
                {
                    if (j == Columns[i].Number) continue;
                    Columns[i][j] -= values[i];
                }
        }

        public List<decimal> MinRowElements()
        {
            var elements = new List<decimal>();
            foreach (var row in Rows)
                elements.Add(row.Min());

            return elements;
        }

        public List<decimal> MinRowWithZeroReplacing()
        {
            var elements = new List<decimal>();

            foreach(var row in Rows)
            {
                decimal min = decimal.MaxValue;
                for (int j = 0; j < row.Values().Count; j++)
                    if (row[j] == 0)
                    {
                        if (j == row.Number) continue;
                        var value = row.Min(j);
                        if (value < min)
                            min = value;
                    }
                if (min == decimal.MaxValue)
                    min = row.Min();
                elements.Add(min);
            }

            return elements;
        }

        public List<decimal> MinColumnElements()
        {
            var elements = new List<decimal>();
            foreach(var column in Columns)
                elements.Add(column.Min());

            return elements;
        }

        public List<decimal> MinColumnWithZeroReplacing()
        {
            var elements = new List<decimal>();

            foreach (var column in Columns)
            {
                decimal min = decimal.MaxValue;
                for (int j = 0; j < column.Values().Count; j++)
                    if (column[j] == 0)
                    {
                        if (j == column.Number) continue;
                        var value = column.Min(j);
                        if (value < min)
                            min = value;
                    }
                if (min == decimal.MaxValue)
                    min = column.Min();
                elements.Add(min);
            }

            return elements;
        }

        public void SetInactiveRow(int index) => Rows[index].IsActive = false;
        public void SetInactiveColumn(int index) => Columns[index].IsActive = false;
        public List<decimal> GetColumn(int columnNumber) => Columns[columnNumber];
        public List<decimal> GetRow(int rowNumber) => Rows[rowNumber];
        public decimal this[int i, int j]
        {
            get
            {
                if (Rows[i].IsActive || Columns[j].IsActive)
                    throw new Exception("Is empty");
                else return Rows[i][j];
            }
            set => Rows[i][j] = value;
        }
    }
}
