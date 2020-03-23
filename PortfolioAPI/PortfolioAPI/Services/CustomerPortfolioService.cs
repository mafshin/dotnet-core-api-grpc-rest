using Grpc.Core;
using PortfolioAPI.Converters.Grpc;
using PortfolioAPI.Data;
using PortfolioAPI.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioAPI.Services
{
    public class CustomerPortfolioService : PortfolioAPI.Grpc.CustomerPortfolioService.CustomerPortfolioServiceBase
    {
        private readonly ICustomerPortfolioRepository _customerPortfolioRepository;

        public CustomerPortfolioService(ICustomerPortfolioRepository customerPortfolioRepository)
        {
            this._customerPortfolioRepository = customerPortfolioRepository;
        }
        public override Task<GetCustomerPortfolioResponse> GetCustomerPortflio(GetCustomerPortfolioRequest request, ServerCallContext context)
        {
            var portoflio = _customerPortfolioRepository.GetCustomerPortfolio(request.CustomerId);

            var grpcPortoflio = portoflio.ConvertToGrpcCustomerPortfolio();

            return Task.FromResult(new GetCustomerPortfolioResponse()
            {
                CustomerPortfolio = grpcPortoflio
            });
        }
    }
}
