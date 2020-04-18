using System;

namespace HttpServer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Введите номер порта:");
            var server = new Server(Console.ReadLine());
            server.Start();
        }
    }
}