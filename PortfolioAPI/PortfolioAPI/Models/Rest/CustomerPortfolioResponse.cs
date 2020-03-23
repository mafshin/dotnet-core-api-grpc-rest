using PortfolioAPI.Models.Common;

namespace PortfolioAPI.Models.Rest
{
    public class CustomerPortfolioResponse
    {
        public CustomerPortfolioResponse()
        {
            CustomerPortfolio = new CustomerPortfolio();
        }
        public CustomerPortfolio CustomerPortfolio { get; set; }
    }
}
