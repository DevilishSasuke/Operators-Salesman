namespace Operators
{
    public abstract class MatrixUnit
    {
        public int Number { get; set; }             // Индекс
        public int ActualNumber { get; set; }       // Начальный номер
        public int Infinity { get; set; }           // Индекс элемента-бесконечности
        public bool IsActive { get; private set; }  // Активен ли элемент
        public Matrix Owner { get; set; }           // Матрица-владелец
        public List<List<decimal>> Distance;        // Список расстояний

        public MatrixUnit(int number, List<List<decimal>> distance, Matrix owner)
        {
            Distance = distance;
            Number = number;
            ActualNumber = number;
            Infinity = Number;
            IsActive = true;
            Owner = owner;
        }

        public decimal Min() => Min(Infinity);
        public decimal Min(int index)
        {
            if (!IsActive) throw new Exception("Inactive");

            var values = Values();
            decimal min = decimal.MaxValue;

            for (int i = 0; i < values.Count; i++)
            {
                if (i == Infinity || i == index) continue;
                if (values[i] < min) min = values[i];
            }

            return min;
        }
        public abstract List<decimal> Values();
        
        public static implicit operator List<decimal>(MatrixUnit unit) => unit.Values();
        public void SetActive() => IsActive = true;
        public void SetInactive() => IsActive = false;
        public abstract decimal this[int index] { get; set; }
    }

    public class Column : MatrixUnit
    {
        public Column(int number, List<List<decimal>> distance, Matrix owner) : base(number, distance, owner) { }

        // Получение значений сущности
        // Только для чтения
        public override List<decimal> Values()
        {
            var values = new List<decimal>();
            if (IsActive)
            {
                foreach (var row in Owner.Rows)
                    if (row.IsActive) values.Add(Distance[row.Number][Number]);
                return values;
            }
            throw new Exception("Inactive column");
        }

        public override decimal this[int index]
        {
            get => Distance[index][Number];
            set => Distance[index][Number] = value;
        }
    }

    public class Row : MatrixUnit
    {
        public Row(int number, List<List<decimal>> distance, Matrix owner) : base(number, distance, owner) { }

        // Получение значений сущности
        // Только для чтения
        public override List<decimal> Values()
        {
            var values = new List<decimal>();
            if (IsActive)
            {
                foreach (var column in Owner.Columns)
                    if (column.IsActive) values.Add(Distance[Number][column.Number]);
                return values;
            }
            throw new Exception("Inactive row");
        }

        public override decimal this[int index]
        {
            get => Distance[Number][index];
            set => Distance[Number][index] = value;
        }
    }
}
