using PortfolioAPI.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioAPI.Data
{
    public class CustomerPortfolioRepository : ICustomerPortfolioRepository
    {
        public CustomerPortfolio GetCustomerPortfolio(int customerId)
        {
            return new CustomerPortfolio()
            {
                CustomerTitle = $"Customer {customerId}",
                Items = Enumerable.Range(0, 10).Select(i => new CustomerPortfolioItem()
                {
                    Count = i * 2,
                    ProfitLoss = -1 * i * i,
                    Stock = $"Stock Number {i}"
                })
            };
        }
    }
}
