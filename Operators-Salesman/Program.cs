namespace Operators
{
    class Program
    {
        public static void Main()
        {
            List<List<decimal>> matrix = new ()
            {
                new (){ 0, 7, 8, 8, 9, 5},
                new (){ 3, 0, 6, 5, 8, 4},
                new (){ 1, 5, 0, 4, 7, 4},
                new (){ 6, 6, 1, 0, 8, 1},
                new (){ 6, 8, 9, 7, 0, 3},
                new (){ 7, 3, 9, 8, 7, 0}
            };

            var salesman = new TravellingSalesman(matrix);
            salesman.FindPath();
        }
    }
}