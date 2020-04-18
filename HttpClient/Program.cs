using System;
using CommonLib;

namespace HttpClient
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Введите номер порта:");
            var dataRep = new DataRepository(Console.ReadLine());
            var input = dataRep.GetInputData();
            var output = ResultCalculator.CalculateOutput(input);
            dataRep.WriteAnswer(output);
        }
    }
}