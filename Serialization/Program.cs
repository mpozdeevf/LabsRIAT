using System;
using System.Linq;
using CommonLib;
using CommonLib.Models;
using CommonLib.Serialize;

namespace Serialization
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Введите тип сериализации (xml, json):");
            var serializationType = Console.ReadLine();
            Console.WriteLine("Введите объект сериализации (Input):");
            var input = Console.ReadLine();
            var output = 
                ResultCalculator.CalculateOutput(UniversalSerializer.Deserialize<Input>(serializationType, input));
            
            Console.WriteLine(UniversalSerializer.Serialize(serializationType, output));
        }
    }
}