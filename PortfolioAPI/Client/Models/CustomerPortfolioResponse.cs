using System.Collections.Generic;

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
    public class CustomerPortfolio
    {
        public string CustomerTitle { get; set; }
        public IEnumerable<CustomerPortfolioItem> Items { get; set; }
    }

    public class CustomerPortfolioItem
    {
        public int Count { get; set; }
        public string Stock { get; set; }
        public decimal ProfitLoss { get; set; }
    }
}
