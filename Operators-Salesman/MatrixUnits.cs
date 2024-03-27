﻿using System.Net;
using System.Text;

namespace Operators
{
    public abstract class MatrixUnit
    {
        public bool IsActive { get; set; }
        public int Number { get; set; }
        public List<List<decimal>> Distance;

        public MatrixUnit(int number, List<List<decimal>> distance)
        {
            Distance = distance;
            Number = number;
            IsActive = true;
        }

        public decimal Min() => Min(Number);
        public decimal Min(int index)
        {
            var values = Values();
            decimal min = decimal.MaxValue;

            for (int i = 0; i < values.Count; i++)
            {
                if (i == Number || i == index) continue;
                if (values[i] < min) min = values[i];
            }

            return min;
        }
        public abstract List<decimal> Values();

        public static implicit operator List<decimal>(MatrixUnit unit) => unit.Values();
        public static implicit operator bool(MatrixUnit unit) => unit.IsActive;
        public static implicit operator int(MatrixUnit unit) => unit.Number;
        public void Add(decimal value) { Values().Add(value); }
        public abstract decimal this[int index] { get; set; }
    }

    public class Column : MatrixUnit
    {
        public Column(int number, List<List<decimal>> distance) : base(number, distance)
        {
        }

        public override List<decimal> Values()
        {
            var values = new List<decimal>();
            if (IsActive)
            {
                foreach (var row in Distance)
                    values.Add(row[Number]);
                return values;
            }
            return null;
        }

        public override decimal this[int index]
        {
            get => Distance[index][Number];
            set => Distance[index][Number] = value;
        }
    }

    public class Row : MatrixUnit
    {
        public Row(int number, List<List<decimal>> distance) : base(number, distance)
        {
        }

        public override List<decimal> Values()
        {
            if (IsActive)
                return Distance[Number];
            return null;
        }

        
        public override decimal this[int index]
        {
            get => Distance[Number][index];
            set => Distance[Number][index] = value;
        }
    }
}
