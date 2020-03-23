using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceAddress = "https://localhost:5001";

            var customerId = 10;

            if(args.Length > 0)
            {
                int.TryParse(args[0], out customerId);
            }

            await TestService(serviceAddress, customerId, ServiceType.Rest);
            await TestService(serviceAddress, customerId, ServiceType.Grpc);

            Console.ReadKey();
        }

        private static async Task TestService(string serviceAddress, int customerId, ServiceType serviceType)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                switch (serviceType)
                {
                    case ServiceType.Rest:
                        await TestRestApi(serviceAddress, customerId);
                        break;
                    case ServiceType.Grpc:
                        await TestGrpcService(serviceAddress, customerId);
                        break;
                }

                sw.Stop();
                Console.WriteLine($">>> Service Call ({serviceType}): {sw.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" !!! Error in REST Call {ex.Message} {ex.StackTrace}");
            }
        }

        private static async Task TestRestApi(string serviceAddress, int customerId)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestUri = string.Format($"{serviceAddress}/api/CustomerPortfolio/{customerId}");

                var response = await CallRestApi(client, requestUri);

                var portfoResponse = response.Item1;

                Console.WriteLine();
                Console.WriteLine("Testing REST Api");
                Console.WriteLine(">>>>>>>>>>>>>>>>>");
                Console.WriteLine($"Request, CustomerId: {customerId}");
                Console.WriteLine($"Response, Length: {response.responseLengh}");
                Console.WriteLine($"Response, Title: {portfoResponse.CustomerPortfolio.CustomerTitle}");
                Console.WriteLine($"Response, Items Count: {portfoResponse.CustomerPortfolio.Items.Count()}");
                Console.WriteLine($"Response, Last Item:");
                Console.WriteLine($"Response, Stock: \t{portfoResponse.CustomerPortfolio.Items.Last().Stock}");
                Console.WriteLine($"Response, Count: \t{portfoResponse.CustomerPortfolio.Items.Last().Count}");
                Console.WriteLine($"Response, ProfitLoss: \t{portfoResponse.CustomerPortfolio.Items.Last().ProfitLoss}");

                for (int i = 0; i < customerId; i++)
                {
                    requestUri = string.Format($"{serviceAddress}/api/CustomerPortfolio/{customerId}");

                    await CallRestApi(client, requestUri);
                }
            }
        }

        private static async Task<(PortfolioAPI.Models.Rest.CustomerPortfolioResponse, long responseLengh)> CallRestApi(HttpClient client, string requestUri)
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

        private static async Task TestGrpcService(string serviceAddress, int customerId)
        {
            using var channel = Grpc.Net.Client.GrpcChannel.ForAddress(serviceAddress);
            var client = new PortfolioAPI.Grpc.CustomerPortfolioService.CustomerPortfolioServiceClient(channel);

            var portfoResponse = await client.GetCustomerPortflioAsync(new PortfolioAPI.Grpc.GetCustomerPortfolioRequest()
            {
                CustomerId = customerId
            });

            Console.WriteLine();
            Console.WriteLine("Testing Grpc Service");
            Console.WriteLine(">>>>>>>>>>>>>>>>>");
            Console.WriteLine($"Request, CustomerId: {customerId}");
            Console.WriteLine($"Response, Length: {portfoResponse.CalculateSize()}");
            Console.WriteLine($"Response, Title: {portfoResponse.CustomerPortfolio.CustomerTitle}");
            Console.WriteLine($"Response, Items Count: {portfoResponse.CustomerPortfolio.Items.Count}");
            Console.WriteLine($"Response, Last Item:");
            Console.WriteLine($"Response, Stock: \t{portfoResponse.CustomerPortfolio.Items.Last().Stock}");
            Console.WriteLine($"Response, Count: \t{portfoResponse.CustomerPortfolio.Items.Last().Count}");
            Console.WriteLine($"Response, ProfitLoss: \t{portfoResponse.CustomerPortfolio.Items.Last().ProfitLoss}");
            
            for(int i=0; i<customerId; i++)
            {
                var tempResponse = await client.GetCustomerPortflioAsync(new PortfolioAPI.Grpc.GetCustomerPortfolioRequest()
                {
                    CustomerId = i
                });
            }
        }
    }

    public enum ServiceType
    {
        Rest = 1,
        Grpc = 2
    }
}
