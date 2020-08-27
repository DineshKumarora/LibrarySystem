using System.Threading.Tasks;
using CustomerAPIService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CustomerAPIService.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Customer Controller to get verify Customer details
    /// </summary>
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly CustomerContext _context;

        /// <summary>
        /// Customer Constructor
        /// </summary>
        /// <param name="context"></param>
        public CustomerController(CustomerContext context)
        {
            _context = context;
        }

        ////GET: api/Customer
        ///// <summary>
        ///// To get all the Customer List
        ///// </summary>
        ///// <returns>Customer List</returns>
        //[Authorize]
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Customer>>> GetCustomerList()
        //{
        //    return await _context.CustomerList.ToListAsync();
        //}

        //GET: api/Customer/{Customer Name}
        /// <summary>
        /// To get specific customer details
        /// </summary>
        /// <param name="customerName">Optional Customer Name</param>
        /// <returns></returns>
        [HttpGet("{customerName}")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomerDetails(string customerName = "")
        {
            List<Customer> customerList;
            if (!string.IsNullOrEmpty(customerName))
            {
                customerList = await _context.CustomerList.Where(x => x.Id == customerName).ToListAsync();

                if (customerList?.Count <= 0)
                    return NotFound();
            }
            else
                customerList = await _context.CustomerList.ToListAsync();

            return Ok(customerList);
        }

        //GET: api/Customer/{user}\{pwd}
        /// <summary>
        /// To verify customer details.
        /// </summary>
        /// <param name="subscriberName">Customer Name</param>
        /// <param name="pwd">password</param>
        /// <returns>Ok or NotFound</returns>
        [HttpGet]
        public IActionResult Get(string subscriberName, string pwd)
        {
            if (string.IsNullOrEmpty(subscriberName) || string.IsNullOrEmpty(pwd)) return NotFound();

            var customerList = _context.CustomerList.Where(x => x.Id == subscriberName && x.Password == pwd).ToListAsync();

            if (customerList.Result.Count > 0)
                return Ok();

            return NotFound();
        }

        /// <summary>
        /// To check the Health of the service
        /// </summary>
        /// <returns>OK</returns>
        [HttpGet]
        [HttpHead]
        [Route("healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        /// <summary>
        /// To get the information about the service
        /// </summary>
        /// <returns>Service Information</returns>
        [HttpGet("info")]
        public string Info()
        {
            return "Customer Service - info";
        }
    }
}

