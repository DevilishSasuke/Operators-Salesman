namespace Operators
{
    public abstract class MatrixUnit
    {
        public bool IsEmpty { get; set; }
        public int Number { get; set; }
        public List<List<decimal>> Distance;

        public MatrixUnit(int number, List<List<decimal>> distance)
        {
            Distance = distance;
            Number = number;
            IsEmpty = false;
        }

        public abstract List<decimal> GetValues();

        public void Add(decimal value) { GetValues().Add(value); }
        public static implicit operator List<decimal>(MatrixUnit unit) => unit.GetValues();
        public static implicit operator bool(MatrixUnit unit) => unit.IsEmpty;
        public static implicit operator int(MatrixUnit unit) => unit.Number;
        public decimal this[int index]
        {
            get => GetValues()[index];
            set => GetValues()[index] = value;
        }
    }

    public class Column : MatrixUnit
    {
        public Column(int number, List<List<decimal>> distance) : base(number, distance)
        {
        }

        public override List<decimal> GetValues()
        {
            var values = new List<decimal>();
            if (!IsEmpty)
            {
                foreach (var row in Distance)
                    values.Add(row[Number]);
                return values;
            }
            return null;
        }
    }

    public class Row : MatrixUnit
    {
        public Row(int number, List<List<decimal>> distance) : base(number, distance)
        {
        }

        public override List<decimal> GetValues()
        {
            if (IsEmpty)
                return null;
            return Distance[Number];
        }
    }
}
