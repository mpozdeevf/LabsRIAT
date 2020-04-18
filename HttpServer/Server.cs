using System;
using System.IO;
using System.Net;
using CommonLib;
using CommonLib.Models;
using CommonLib.Serialize;

namespace HttpServer
{
    public class Server
    {
        private const string PingAddress = "/Ping/";
        private const string PostInputDataAddress = "/PostInputData/";
        private const string GetAnswerAddress = "/GetAnswer/";
        private const string StopAddress = "/Stop/";
        private const string GetInputDataAddress = "/GetInputData/";
        private const string WriteAnswerAddress = "/WriteAnswer/";

        private readonly string _url;
        private readonly HttpListener _httpListener;

        public Server(string port)
        {
            _httpListener = new HttpListener();
            _url = $"http://127.0.0.1:{port}";

            _httpListener.Prefixes.Add($"{_url}{PingAddress}");
            _httpListener.Prefixes.Add($"{_url}{PostInputDataAddress}");
            _httpListener.Prefixes.Add($"{_url}{GetAnswerAddress}");
            _httpListener.Prefixes.Add($"{_url}{StopAddress}");
            _httpListener.Prefixes.Add($"{_url}{GetInputDataAddress}");
            _httpListener.Prefixes.Add($"{_url}{WriteAnswerAddress}");
        }

        public void Start()
        {
            var input = new Input();

            _httpListener.Start();
            Console.WriteLine($"Server now listening on {_url}");
            while (_httpListener.IsListening)
            {
                var context = _httpListener.GetContext();
                var request = context.Request;

                switch (request.Url.LocalPath)
                {
                    case PingAddress:
                        Ping(context);
                        break;
                    case PostInputDataAddress:
                        input = PostInputData(context);
                        break;
                    case GetAnswerAddress:
                        var output = ResultCalculator.CalculateOutput(input);
                        GetAnswer(context, output);
                        break;
                    case GetInputDataAddress:
                        GetInputData(context, input);
                        break;
                    case WriteAnswerAddress:
                        WriteAnswer(context, input);
                        break;
                    case StopAddress:
                        Stop(context);
                        break;
                }
            }
        }

        private static void Ping(HttpListenerContext context)
        {
            using var sw = new StreamWriter(context.Response.OutputStream);
            sw.Write(HttpStatusCode.OK);

            context.Response.StatusCode = 200;
        }

        private static Input PostInputData(HttpListenerContext context)
        {
            using var sr = new StreamReader(context.Request.InputStream);
            using var sw = new StreamWriter(context.Response.OutputStream);
            var obj = sr.ReadToEnd();
            try
            {
                var input = UniversalSerializer.Deserialize<Input>(SerializationType.Json, obj);
                sw.Write(UniversalSerializer.Serialize(SerializationType.Json, input));
                
                return input;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                sw.Write(HttpStatusCode.BadRequest);

                return null;
            }
        }

        private static void GetAnswer(HttpListenerContext context, Output output)
        {
            using var sw = new StreamWriter(context.Response.OutputStream);
            sw.Write(UniversalSerializer.Serialize(SerializationType.Json, output));
        }

        private static void GetInputData(HttpListenerContext context, Input input)
        {
            using var sw = new StreamWriter(context.Response.OutputStream);
            sw.Write(UniversalSerializer.Serialize(SerializationType.Json, input));
        }

        private static void WriteAnswer(HttpListenerContext context, Input input)
        {
            using var sr = new StreamReader(context.Request.InputStream);
            using var sw = new StreamWriter(context.Response.OutputStream);
            var clientOutputJson = sr.ReadToEnd();
            var serverOutputJson = 
                UniversalSerializer.Serialize(SerializationType.Json, ResultCalculator.CalculateOutput(input));

            if (serverOutputJson == clientOutputJson)
            {
                sw.Write("Correct answer");
            }
            else
            {
                sw.Write("Incorrect answer");
            }
        }

        private void Stop(HttpListenerContext context)
        {
            const string stopMsg = "Server stopped";
            using (var sw = new StreamWriter(context.Response.OutputStream))
            {
                sw.Write(stopMsg);
            }

            Console.WriteLine(stopMsg);
            _httpListener.Stop();
        }
    }
}