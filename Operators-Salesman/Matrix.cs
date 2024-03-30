namespace Operators
{
    public class Matrix
    {
        public List<Row> Rows { get; set; }
        public List<Column> Columns { get; set; }
        public List<List<decimal>> Distance { get; private set; }

        // Создание матрицы
        public Matrix(List<List<decimal>> distance)
        {
            Rows = new List<Row>();
            Columns = new List<Column>();
            Distance = new();

            foreach (var row in distance)
                Distance.Add(new(row));

            for (int i = 0; i < Distance.Count; i++)
            {
                Rows.Add(new Row(i, Distance, this));
                Columns.Add(new Column(i, Distance, this));
            }
        }

        // Нахождение следующего пункта пути
        public (int, int) FindNextEdge(ref decimal bound)
        {
            // Находим минимальные элементы с поочередной заменой нулей
            var rowConsts = MinRowWithZeroReplacing();
            var columnConsts = MinColumnWithZeroReplacing();

            // Находим максимальную сумму
            var (indexI, indexJ) = MaxIntersection(rowConsts, columnConsts);
            var excluded = ExcludeEdge(indexI, indexJ); // Сдвиг при исключении ребра
            var included = IncludeEdge(indexI, indexJ); // Сдвиг при включении ребра

            if (included <= excluded)
            {
                bound += included;
                indexI = Rows[indexI].ActualNumber;
                indexJ = Columns[indexJ].ActualNumber;
                CreateNewMatrix();
                return (indexI, indexJ);
            }

            bound += excluded;
            RollBack(indexI, indexJ);
            throw new Exception("Incorrect result");
        }

        // Исключение ребра
        public decimal ExcludeEdge(int i, int j)
        {
            // Пересчёт констант со смещением бесконечностей
            var rowConsts = MinRowElements();
            rowConsts[i] = Rows[i].Min(j);
            var columnConsts = MinColumnElements();
            columnConsts[j] = Columns[j].Min(i);

            return rowConsts.Sum() + columnConsts.Sum();
        }

        // Включение ребра
        public decimal IncludeEdge(int i, int j)
        {
            SetInactive(Rows[i]);
            SetInactive(Columns[j]);

            var rowConsts = MinRowElements();
            var columnConsts = MinColumnElements();

            return rowConsts.Sum() + columnConsts.Sum();
        }

        // Расстановка элементов-бесконечностей в матрице
        public void SetInfinities(Dictionary<int, int> edges)
        {
            List<(int, int)> infinities;
            if (edges.Count < 2) return;

            infinities = FindRestrictions(edges);

            foreach(var infinity in infinities)
                SetRestriction(infinity);
        }

        // Постановка бесконечности в верное расположение в новой матрице
        public void SetRestriction((int, int) infinity)
        {
            var rowIndex = infinity.Item1;
            var columnIndex = infinity.Item2;
            int toSwitch;
            // Выставляем элемент-бесконечноcть относительно новой матрицы
            foreach (var row in Rows)
                if (row.ActualNumber == rowIndex)
                {
                    toSwitch = ActualToLocalColumn(columnIndex);
                    if (toSwitch < 0) continue;
                    SwitchRowInfinity(toSwitch, row.Infinity);
                    row.Infinity = toSwitch;
                }
            foreach (var column in Columns)
                if (column.ActualNumber == columnIndex)
                {
                    toSwitch = ActualToLocalRow(rowIndex);
                    if (toSwitch < 0) continue;
                    SwitchColumnInfinity(toSwitch, column.Infinity);
                    column.Infinity = toSwitch;
                }
        }

        // Наибольшая сумма констант
        public (int, int) MaxIntersection(List<decimal> rowsConsts, List<decimal> columnsConsts)
        {
            decimal value, maxValue = decimal.MinValue;
            int resI = 0, resJ = 0;

            foreach (var row in Rows.Where(x => x.IsActive))
                foreach (var column in Columns.Where(x => x.IsActive))
                    if (Distance[row.Number][column.Number] == 0
                        && row.Number != column.Infinity
                        && column.Number != row.Infinity)
                    {
                        value = rowsConsts[row.Number] + columnsConsts[column.Number];
                        if (value > maxValue)
                        {
                            maxValue = value;
                            (resI, resJ) = (row.Number, column.Number);
                        }
                    }

            return (resI, resJ);
        }

        // Получить ограничения для исключения подциклов
        public List<(int, int)> FindRestrictions(Dictionary<int, int> edges)
        {
            var infinities = new List<(int, int)>();

            foreach (var path in edges)
                foreach (var otherPath in edges)
                    if (path.Value == otherPath.Key)
                        infinities.Add((otherPath.Value, path.Key));

            return infinities;
        }

        // Минимальные элементы каждого из рядов
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

            foreach(var row in Rows.Where(x => x.IsActive))
            {
                decimal min = decimal.MaxValue;
                foreach(var column in Columns.Where(x => x.IsActive))
                    if (row[column.Number] == 0)
                    {
                        if (column.Number == row.Infinity) continue;
                        var value = row.Min(column.Number);
                        if (value < min)
                            min = value;
                    }
                if (min == decimal.MaxValue)
                    min = row.Min();
                elements.Add(min);
            }

            return elements;
        }

        // Минимальные элементы каждого из столбцов
        public List<decimal> MinColumnElements()
        {
            var elements = new List<decimal>();
            foreach (var column in Columns)
                if (column.IsActive) elements.Add(column.Min());

            return elements;
        }

        // Поочередное исключение нолей в столбцах
        public List<decimal> MinColumnWithZeroReplacing()
        {
            var elements = new List<decimal>();

            foreach(var column in Columns.Where(x => x.IsActive))
            {
                decimal min = decimal.MaxValue;
                foreach(var row in Rows.Where(x => x.IsActive))
                    if (column[row.Number] == 0)
                    {
                        if (row.Number == column.Infinity) continue;
                        var value = column.Min(row.Number);
                        if (value < min)
                            min = value;
                    }
                if (min == decimal.MaxValue)
                    min = column.Min();
                elements.Add(min);
            }

            return elements;
        }

        // Создание новой матрицы для упрощения навигации
        public void CreateNewMatrix()
        {
            // Новые значения для матрицы
            var newDistance = Rows
                .Where(x => x.IsActive)
                .Select(x => x.Values()).ToList();
            // Изначальные номера рядов и столбцов
            var rowActualNumbers = Rows
                .Where(x => x.IsActive)
                .Select(x => x.ActualNumber).ToList();
            var columnActualNumbers = Columns
                .Where(x => x.IsActive)
                .Select(x => x.ActualNumber).ToList();

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

        // Нижнее значение границы
        public decimal LowerBoundValue()
        {
            var rowElements = MinRowElements();
            SubInRows(rowElements);
            var columnElements = MinColumnElements();
            SubInColumns(columnElements);

            return rowElements.Sum() + columnElements.Sum();
        }

        public void SwitchRowInfinity(int toSwitch, int value)
        {
            foreach (var row in Rows)
                if (row.Infinity == toSwitch)
                    row.Infinity = value;
        }

        public void SwitchColumnInfinity(int toSwitch, int value)
        {
            foreach (var column in Columns)
                if (column.Infinity == toSwitch)
                    column.Infinity = value;
        }

        public void SubInRows(List<decimal> values)
        {
            for (int i = 0; i < Rows.Count; i++)
                for (int j = 0; j < Columns.Count; j++)
                {
                    if (j == Rows[i].Infinity) continue;
                    Rows[i][j] -= values[i];
                }
        }

        public void SubInColumns(List<decimal> values)
        {
            for (int j = 0; j < Columns.Count; j++)
                for (int i = 0; i < Rows.Count; i++)
                {
                    if (i == Columns[j].Infinity) continue;
                    Columns[j][i] -= values[j];
                }
        }

        // Нахождение индекса ряда по его настоящему номеру
        public int ActualToLocalRow(int index)
        {
            var number = Rows
                .Where(x => x.ActualNumber == index)
                .Select(x => x.Number);
            return number.Count() == 0 ? -1 : number.First();
        }

        // Нахождение индекса столбца по его настоящему номеру
        public int ActualToLocalColumn(int index)
        {
            var number = Columns
                .Where(x => x.ActualNumber == index)
                .Select(x => x.Number);
            return number.Count() == 0 ? -1 : number.First();
        }

        // Смена активных рядов и столбцов
        private void SetActive(MatrixUnit unit) => unit.SetActive();
        private void SetInactive(MatrixUnit unit) => unit.SetInactive();
        public int RowsCount() => Rows.Where(x => x.IsActive).Count();
        public int ColumnsCount() => Columns.Where(x => x.IsActive).Count();
        private void RollBack(int i, int j) { SetActive(Rows[i]); SetActive(Columns[j]); }
    }
}
