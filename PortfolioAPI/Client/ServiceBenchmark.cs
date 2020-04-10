using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    [MemoryDiagnoser]  
    public class ServiceBenchmark
    {
        private Grpc.Net.Client.GrpcChannel channel = null;
        private HttpClient restClient = null;

        private string serviceAddress;


        [GlobalSetup]
        public void Setup()
        {
            serviceAddress = "https://localhost:5001";

            channel = Grpc.Net.Client.GrpcChannel.ForAddress(serviceAddress);
            restClient = HttpClientFactory.Create();
        }

        [Benchmark(Baseline = true)]
        [Arguments(2000)]        
        public async Task RESTApi(int count) => await TestService(ServiceType.Rest, count);


        [Benchmark]
        [Arguments(2000)]
        public async Task GrpcApi(int count) => await TestService(ServiceType.Grpc, count);

        private async Task TestService(ServiceType serviceType, int count)
        {
            var tasks = new List<Task>();
            for (int customerId = 0; customerId < count; customerId++)
            {
                switch (serviceType)
                {
                    case ServiceType.Rest:
                        tasks.Add(TestRestApi(customerId, false));
                        break;
                    case ServiceType.Grpc:
                        tasks.Add(TestGrpcService(customerId));
                        break;
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task TestRestApi(int customerId, bool printOutput = false)
        {
            var requestUri = string.Format($"{serviceAddress}/api/CustomerPortfolio/{customerId}");

            var response = await CallRestApi(restClient, requestUri);

            if (response.Portfolio == null)
            {
                throw new Exception("REST Response is null");
            }

            if (printOutput)
            {
                var portfoResponse = response.Portfolio;

                Console.WriteLine();
                Console.WriteLine("Testing REST Api");
                Console.WriteLine(">>>>>>>>>>>>>>>>>");
                Console.WriteLine($"Request, CustomerId: {customerId}");
                Console.WriteLine($"Response Length: {response.responseLengh}");
                Console.WriteLine($"Response Details");
                Console.WriteLine($" > Title: {portfoResponse.CustomerPortfolio.CustomerTitle}");
                Console.WriteLine($" > Items Count: {portfoResponse.CustomerPortfolio.Items.Count()}");
                Console.WriteLine($" > Last Portoflio Item:");
                Console.WriteLine($" >> Stock: \t{portfoResponse.CustomerPortfolio.Items.Last().Stock}");
                Console.WriteLine($" >> Count: \t{portfoResponse.CustomerPortfolio.Items.Last().Count}");
                Console.WriteLine($" >> ProfitLoss: \t{portfoResponse.CustomerPortfolio.Items.Last().ProfitLoss}");
            }
        }

        private async Task<(PortfolioAPI.Models.Rest.CustomerPortfolioResponse Portfolio, long responseLengh)> CallRestApi(HttpClient client, string requestUri)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, requestUri)
            {
                Version = new Version(2, 0)
            };

            var response = await client.SendAsync(req);

            var responseString = await response.Content.ReadAsStringAsync();

            var portfoResponse = System.Text.Json.JsonSerializer.Deserialize<PortfolioAPI.Models.Rest.CustomerPortfolioResponse>(responseString, new System.Text.Json.JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            var responseLength = response.Content.Headers.ContentLength.GetValueOrDefault();

            return (portfoResponse, responseLength);
        }

        private async Task TestGrpcService(int customerId, bool printOutput = false)
        {
            var client = new PortfolioAPI.Grpc.CustomerPortfolioService.CustomerPortfolioServiceClient(channel);

            var portfoResponse = await client.GetCustomerPortflioAsync(new PortfolioAPI.Grpc.GetCustomerPortfolioRequest()
            {
                CustomerId = customerId
            });

            if (portfoResponse == null)
            {
                throw new Exception("Grpc Response is null");
            }

            if (printOutput)
            {
                Console.WriteLine();
                Console.WriteLine("Testing Grpc Service");
                Console.WriteLine(">>>>>>>>>>>>>>>>>");
                Console.WriteLine($"Request, CustomerId: {customerId}");
                Console.WriteLine($"Response Length: {portfoResponse.CalculateSize()}");
                Console.WriteLine($"Response Details");
                Console.WriteLine($" > Title: {portfoResponse.CustomerPortfolio.CustomerTitle}");
                Console.WriteLine($" > Items Count: {portfoResponse.CustomerPortfolio.Items.Count}");
                Console.WriteLine($" > Last Portfolio Item:");
                Console.WriteLine($" >> Stock: \t{portfoResponse.CustomerPortfolio.Items.Last().Stock}");
                Console.WriteLine($" >> Count: \t{portfoResponse.CustomerPortfolio.Items.Last().Count}");
                Console.WriteLine($" >> ProfitLoss: \t{portfoResponse.CustomerPortfolio.Items.Last().ProfitLoss}");
            }
        }

        public enum ServiceType
        {
            Rest = 1,
            Grpc = 2
        }
    }
}
