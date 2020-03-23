using PortfolioAPI.Grpc;
using System.Linq;

namespace PortfolioAPI.Converters.Grpc
{
    public static class CustomerPortfolioConverters
    {
        public static PortfolioAPI.Grpc.CustomerPortfolio ConvertToGrpcCustomerPortfolio(this PortfolioAPI.Models.Common.CustomerPortfolio customerPortfolio)
        {
            var grpcCustomerPortfolio = new CustomerPortfolio()
            {
                CustomerTitle = customerPortfolio.CustomerTitle
            };

            grpcCustomerPortfolio.Items.AddRange(customerPortfolio.Items.Select(p => new CustomerPortfolioItem()
            {
                Count = p.Count,
                ProfitLoss = (long)p.ProfitLoss,
                Stock = p.Stock
            }));

            return grpcCustomerPortfolio;
        }
    }
}
