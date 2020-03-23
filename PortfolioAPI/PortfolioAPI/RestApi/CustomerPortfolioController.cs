using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.Converters.Rest;
using PortfolioAPI.Data;

namespace PortfolioAPI.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerPortfolioController : ControllerBase
    {
        private readonly ICustomerPortfolioRepository _customerPortfolioRepository;

        public CustomerPortfolioController(ICustomerPortfolioRepository customerPortfolioRepository)
        {
            this._customerPortfolioRepository = customerPortfolioRepository;
        }

        [HttpGet("{customerId}")]
        public PortfolioAPI.Models.Rest.CustomerPortfolioResponse Get(int customerId)
        {
            var portfo = _customerPortfolioRepository.GetCustomerPortfolio(customerId);
            var response = portfo.ConvertToGrpcCustomerPortfolio();
            return response;
        }
    }
}