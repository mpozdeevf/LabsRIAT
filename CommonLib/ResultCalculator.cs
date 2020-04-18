using System.Linq;
using CommonLib.Models;

namespace CommonLib
{
    public static class ResultCalculator
    {
        public static Output CalculateOutput(Input input)
        {
            if (input.Muls == null || input.Sums == null)
            {
                return new Output();
            }
            
            return new Output
            {
                SumResult = input.Sums.Sum() * input.K,
                MulResult = input.Muls.Aggregate(1, (x, y) => x * y),
                SortedInputs = input.Sums
                    .Union(input.Muls.Select(x => (decimal) x))
                    .OrderBy(x => x)
                    .ToArray()
            };
        }
    }
}