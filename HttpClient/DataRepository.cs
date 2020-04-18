using System;
using System.Net.Http;
using System.Text;
using CommonLib.Models;
using CommonLib.Serialize;

namespace HttpClient
{
    public class DataRepository
    {
        private const string GetInputDataMethod = "/GetInputData/";
        private const string WriteAnswerMethod = "/WriteAnswer/";
        private const string PingMethod = "/Ping/";

        private readonly string _serverAddress;
        
        private readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

        public DataRepository(string port)
        {
            _serverAddress = $"http://127.0.0.1:{port}";
        }

        public Input GetInputData()
        {
            if (!IsServerAvailable())
            {
                Console.WriteLine($"Server {_serverAddress} not available");
                
                return null;
            }
            
            var response = _httpClient.GetAsync($"{_serverAddress}{GetInputDataMethod}").Result;
            try
            {
                response.EnsureSuccessStatusCode();
                var jsonInputData = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"Input data {jsonInputData} received");
                
                return UniversalSerializer.Deserialize<Input>(SerializationType.Json, jsonInputData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                return null;
            }
        }

        public void WriteAnswer(Output output)
        {
            if (!IsServerAvailable())
            {
                Console.WriteLine($"Server {_serverAddress} not available");
            }

            var jsonOutput = UniversalSerializer.Serialize(SerializationType.Json, output);
            var content = new StringContent(jsonOutput, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync($"{_serverAddress}{WriteAnswerMethod}", content).Result;
            try
            {
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"Answer {jsonOutput} sent");
                Console.WriteLine($"Response: {response.Content.ReadAsStringAsync().Result}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private bool IsServerAvailable()
        {
            var response = _httpClient.GetAsync($"{_serverAddress}{PingMethod}");

            return response.Result.IsSuccessStatusCode;
        }
    }
}