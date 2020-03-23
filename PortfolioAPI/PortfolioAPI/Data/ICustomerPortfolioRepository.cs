using PortfolioAPI.Models.Common;

namespace PortfolioAPI.Data
{
    public interface ICustomerPortfolioRepository
    {
        CustomerPortfolio GetCustomerPortfolio(int customerId);
    }
}