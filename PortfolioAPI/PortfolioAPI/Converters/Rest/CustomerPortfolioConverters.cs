using PortfolioAPI.Models.Common;
using System.Linq;

namespace PortfolioAPI.Converters.Rest
{
    public static class CustomerPortfolioConverters
    {
        public static PortfolioAPI.Models.Rest.CustomerPortfolioResponse ConvertToGrpcCustomerPortfolio(this PortfolioAPI.Models.Common.CustomerPortfolio customerPortfolio)
        {
            var restCustomerPortfolio = new PortfolioAPI.Models.Rest.CustomerPortfolioResponse();            
            restCustomerPortfolio.CustomerPortfolio.CustomerTitle = customerPortfolio.CustomerTitle;

            restCustomerPortfolio.CustomerPortfolio.Items =
                customerPortfolio.Items.Select(p => new CustomerPortfolioItem()
                {
                    Count = p.Count,
                    ProfitLoss = (long)p.ProfitLoss,
                    Stock = p.Stock
                });

            return restCustomerPortfolio;
        }
    }
}
