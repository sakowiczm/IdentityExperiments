using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Service02
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Service 02";

            Task.Run(() => StartCalling()).Wait();

            Console.ReadLine();
        }

        public static async void StartCalling()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            if (disco.IsError)
                throw new Exception(disco.Error);

            var discoClient = new TokenClient(disco.TokenEndpoint, "console-client", "secret");
            var authenticationResponse = await discoClient.RequestClientCredentialsAsync("service1-api.get-datetime");

            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5002/")
            };

            client.SetBearerToken(authenticationResponse.AccessToken);

            int count = 1;
            while (true)
            {
                Console.WriteLine($"Call {count}");

                var response = await client.GetStringAsync("api/values/getdatetime");
                Console.WriteLine(response);

                Console.WriteLine("------------------------------------");

                Thread.Sleep(1000);
                count++;
            }
        }
    }
}