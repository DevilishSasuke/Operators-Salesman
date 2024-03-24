using System.Runtime.CompilerServices;

namespace Operators
{
    internal class MatrixUnit
    {
        public bool IsEmpty { get; set; }
        public int Number { get; set; }
        private List<decimal> values;
        public List<decimal> Values
        {
            get => IsEmpty ? null : values;
            set => values = value;
        }

        public MatrixUnit(int number)
        {
            Values = new List<decimal>();
            Number = number;
            IsEmpty = false;
        }

        public MatrixUnit(List<decimal> values, int number)
        {
            Values = values;
            Number = number;
            IsEmpty = false;
        }

        public void Add(decimal value) { Values.Add(value); }
        public static implicit operator List<decimal>(MatrixUnit unit) => unit.Values;
        public static implicit operator bool(MatrixUnit unit) => unit.IsEmpty;
        public static implicit operator int(MatrixUnit unit) => unit.Number;
        public decimal this[int index]
        {
            get => values[index];
            set => values[index] = value;
        }
    }
}
