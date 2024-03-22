namespace Operators
{
    class Program
    {
        public static void Main()
        {
            decimal[][] matrix =
            {
                new decimal[]{ 0, 7, 8, 8, 9, 5},
                new decimal[]{ 3, 0, 6, 5, 8, 4},
                new decimal[]{ 1, 5, 0, 4, 7, 4},
                new decimal[]{ 6, 6, 1, 0, 8, 1},
                new decimal[]{ 6, 8, 9, 7, 0, 3},
                new decimal[]{ 7, 3, 9, 8, 7, 0}
            };

            var salesman = new TravellingSalesman(matrix);
        }
    }
}