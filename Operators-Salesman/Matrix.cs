using System.Runtime.InteropServices;

namespace Operators
{
    public class Matrix
    {
        public List<Column> Columns { get; set; }
        public List<Row> Rows { get; set; }
        public List<List<decimal>> Distance { get; private set; }
        private Dictionary<int, int> edges;
        private List<Row> RemovedRows = new();
        private List<Column> RemovedColumns = new();

        public Matrix(List<List<decimal>> distance)
        {
            Rows = new List<Row>();
            Columns = new List<Column>();
            edges = new();
            Distance = new();
            foreach (var row in distance)
                Distance.Add(new(row));
            for (int i = 0; i < Distance.Count; i++)
            {
                Rows.Add(new Row(i, Distance, this));
                Columns.Add(new Column(i, Distance, this));
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

        public (int, int) FindNextEdge(ref decimal bound)
        {
            var rowElements = MinRowWithZeroReplacing();
            var columnElements = MinColumnWithZeroReplacing();

            var (indexI, indexJ) = MaxIntersection(rowElements, columnElements);
            var excluded = ExcludeEdge(indexI, indexJ);
            var included = IncludeEdge(indexI, indexJ);
            if (included <= excluded)
            {
                bound += included;
                indexI = Rows[indexI].ActualNumber;
                indexJ = Columns[indexJ].ActualNumber;
                CreateNewMatrix();
                return (indexI, indexJ);
            }
            else
            {
                bound += excluded;
                RollBack(indexI, indexJ);
                throw new Exception("Incorrect result");
            }
        }

        public (int, int) MaxIntersection(List<decimal> rows, List<decimal> columns)
        {
            decimal maxValue = decimal.MinValue, value;
            int resI = 0, resJ = 0;

            for (int i = 0; i < Distance.Count; i++)
            {
                if (!Rows[i].IsActive) continue;
                for (int j = 0; j < Distance[0].Count; j++)
                {
                    if (!Columns[j].IsActive) continue;
                    if (Distance[i][j] == 0 && j != Rows[i].Blocked && i != Columns[j].Blocked)
                    {
                        value = rows[i] + columns[j];
                        if (value > maxValue)
                        {
                            maxValue = value;
                            (resI, resJ) = (i, j);
                        }
                    }

                }
            }

            return (resI, resJ);
        } 

        public void CreateNewMatrix()
        {
            var newDistance = new List<List<decimal>>();
            List<int> rowActualNumbers = new(), columnActualNumbers = new();

            foreach (var column in Columns)
                if (column.IsActive)
                    columnActualNumbers.Add(column.ActualNumber);

            foreach (var row in Rows)
                if (row.IsActive)
                {
                    newDistance.Add(row.Values());
                    rowActualNumbers.Add(row.ActualNumber);
                }

            var matrix = new Matrix(newDistance);
            this.Distance = matrix.Distance;
            this.Rows = matrix.Rows;
            this.Columns = matrix.Columns;

            for (int i = 0; i < Distance.Count; i++)
            {
                Rows[i].ActualNumber = rowActualNumbers[i];
                Columns[i].ActualNumber = columnActualNumbers[i];
            }
        }

        public decimal ExcludeEdge(int indexI, int indexJ)
        {
            var rowElements = MinRowElements();
            rowElements[indexI] = Rows[indexI].Min(indexJ);
            var columnElements = MinColumnElements();
            columnElements[indexJ] = Columns[indexJ].Min(indexI);

            return rowElements.Sum() + columnElements.Sum();
        }

        public decimal IncludeEdge(int i, int j)
        {
            SetInactiveRow(i);
            SetInactiveColumn(j);

            var rowElements = MinRowElements();
            var columnElements = MinColumnElements();

            return rowElements.Sum() + columnElements.Sum();
        }

        public void SubInRows(List<decimal> values)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                {
                    if (j == Rows[i].Number) continue;
                    Rows[i][j] -= values[i];
                }
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
                if (row.IsActive) elements.Add(row.Min());

            return elements;
        }

        // Поочередное исключение нолей в рядах
        public List<decimal> MinRowWithZeroReplacing()
        {
            var elements = new List<decimal>();

            foreach (var row in Rows)
            {
                if (!row.IsActive) continue;
                decimal min = decimal.MaxValue;
                for (int j = 0; j < row.Values().Count; j++) 
                {
                    if (!Columns[j].IsActive) continue;
                    if (row[j] == 0)
                    {
                        if (j == row.Blocked) continue;
                        var value = row.Min(j);
                        if (value < min)
                            min = value;
                    }
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
                if (column.IsActive) elements.Add(column.Min());

            return elements;
        }

        // Поочередное исключение нолей в столбцах
        public List<decimal> MinColumnWithZeroReplacing()
        {
            var elements = new List<decimal>();

            foreach (var column in Columns)
            {
                if (!column.IsActive) continue;
                decimal min = decimal.MaxValue;
                for (int j = 0; j < column.Values().Count; j++) 
                {
                    if (!Rows[j].IsActive) continue;
                    if (column[j] == 0)
                    {
                        if (j == column.Blocked) continue;
                        var value = column.Min(j);
                        if (value < min)
                            min = value;
                    }
                }
                if (min == decimal.MaxValue)
                    min = column.Min();
                elements.Add(min);
            }

            return elements;
        }

        public void SetInfinities(Dictionary<int, int> edges)
        {
            List<(int, int)> blocks = new List<(int, int)>();
            if (edges.Count < 2) return;

            foreach(var items in edges)
                foreach(var otherItems in edges)
                    if (items.Value == otherItems.Key)
                        blocks.Add((otherItems.Value, items.Key));

            foreach(var block in blocks)
            {
                var rowIndex = block.Item1;
                var columnIndex = block.Item2;
                int toSwitch;
                foreach (var row in Rows)
                    if (row.ActualNumber == rowIndex)
                    {
                        toSwitch = ActualToLocalColumn(columnIndex);
                        SwitchRowBlocked(toSwitch, row.Blocked);
                        row.Blocked = toSwitch;
                    }
                foreach (var column in Columns)
                    if (column.ActualNumber == columnIndex)
                    {
                        toSwitch = ActualToLocalRow(rowIndex);
                        SwitchColumnBlocked(toSwitch, column.Blocked);
                        column.Blocked = toSwitch;
                    }
            }
        }

        public void SwitchRowBlocked(int toSwitch, int value)
        {
            foreach (var row in Rows)
                if (row.Blocked == toSwitch)
                    row.Blocked = value;
        }

        public void SwitchColumnBlocked(int toSwitch, int value)
        {
            foreach (var column in Columns)
                if (column.Blocked == toSwitch)
                    column.Blocked = value;
        }

        public int ActualToLocalColumn(int index)
        {
            foreach (var column in Columns)
                if (column.ActualNumber == index)
                    return column.Number;

            throw new Exception("Not found");
        }

        public int ActualToLocalRow(int index)
        {
            foreach (var row in Rows)
                if (row.ActualNumber == index)
                    return row.Number;

            throw new Exception("Not found");
        }

        private void RememberRemovedRow(Row row)
        {
            int offset = 0;
            for (int i = RemovedRows.Count - 1; i >= 0; i--)
                if (row.Number + offset >= RemovedRows[i].Number)
                    offset++;
            row.Number += offset;
            RemovedRows.Add(row);
        }
        private void RememberRemovedColumn(Column column)
        {
            int offset = 0;
            for (int i = RemovedColumns.Count - 1; i >= 0; i--)
                if (column.Number + offset >= RemovedColumns[i].Number)
                    offset++;
            column.Number += offset;
            RemovedColumns.Add(column);
        }

        private void RollBack(int i, int j)
        {
            SetActiveRow(i);
            SetActiveColumn(j);
        }

        private void SetActiveRow(int index) => Rows[index].SetActive();
        public void SetInactiveRow(int index) => Rows[index].SetInactive();
        private void SetActiveColumn(int index) => Columns[index].SetActive();
        public void SetInactiveColumn(int index) => Columns[index].SetInactive();
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

        public int RowsCount()
        {
            int count = 0;
            foreach (var row in Rows)
                if (row.IsActive) count++;

            return count;
        }

        public int ColumnsCount() 
        {
            int count = 0;
            foreach(var column in Columns)
                if (column.IsActive) count++;

            return count;
        }
    }
}
