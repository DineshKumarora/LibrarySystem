using System;
using System.Threading;
using System.Threading.Tasks;
using SubscriptionAPIService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CustomerAPIService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace SubscriptionAPIService.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    public class SubscriptionController : Controller
    {
        private readonly SubscriptionContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public SubscriptionController(SubscriptionContext context)
        {
            _context = context;
        }

        //GET: api/Subscription
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptionLists()
        {
            return await _context.SubscriptionList.ToListAsync();
        }

        //GET: api/Subscription/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriberName"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{subscriberName}")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions(string subscriberName)
        {
            List<Subscription> subscriptionList;
            if (!string.IsNullOrEmpty(subscriberName))
            {
                subscriptionList = await _context.SubscriptionList.Where(x => x.SubscriberName == subscriberName)
                    .ToListAsync();

                if (subscriptionList?.Count <= 0)
                    return NotFound();
            }
            else
                subscriptionList = await _context.SubscriptionList.ToListAsync();

            return Ok(subscriptionList);
        }

        // POST: api/Subscriptions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Subscription>> PostSubscriptions([FromBody]Subscription subscription)
        {
            using (var client = new HttpClient())
            {
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", this.Request.Headers["Authorization"].ToString());
                
                client.BaseAddress = new Uri("http://localhost:62793/");

                try
                {
                    //Update Availability
                    var response = client.PutAsync($"/Books/{subscription.BookId}", new StringContent(subscription.BookId, Encoding.UTF8, "application/json")).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        _context.SubscriptionList.Add(subscription);
                        await _context.SaveChangesAsync();

                        string jsonMessage = response.Content.ReadAsStringAsync().Result;

                        // De-serialize
                        var bookDetail = (Books)
                            JsonConvert.DeserializeObject(jsonMessage,
                                typeof(Books));

                        if (bookDetail?.copiesAvailable <= 0 && subscription.Notify)
                        {
                            // Get the Customer details and email address
                            //client.DefaultRequestHeaders.Clear();
                            response = client.GetAsync($"/Customer/{subscription.SubscriberName}").Result;
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                jsonMessage = response.Content.ReadAsStringAsync().Result;

                                // De-serialize
                                var customer = (Customer)
                                    JsonConvert.DeserializeObject(jsonMessage,
                                        typeof(Customer));

                                //Add the alert for the cuscriber as user opted for Notification of availability
                                //client.DefaultRequestHeaders.Clear();
                                string registerUserJson = RegisterUserJson(subscription.BookId, subscription.Id,
                                    customer.Email);
                                HttpRequestMessage request =
                                    new HttpRequestMessage(HttpMethod.Post, client.BaseAddress.ToString())
                                    {
                                        Content = new StringContent(registerUserJson, Encoding.UTF8, "application/json")
                                    };
                                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                response = client.PostAsync($"/AlertSubscription", request.Content).Result;
                            }
                        }

                        //Send alert to all user as books are available
                        if (bookDetail?.copiesAvailable > 0)
                        {
                            //client.DefaultRequestHeaders.Clear();
                            response = client.GetAsync($"/AlertSubscription/{subscription.BookId}").Result;
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }

            return CreatedAtAction(nameof(GetSubscriptionLists), new
            {
                id = subscription.BookId
            }, subscription);
        }

        private string RegisterUserJson(string bookId, string subscriberName, string email)
        {
            string registerUserJSON = "{"
                                      //+ "Subscription\": {"
                                      + "\"BookId\": \"" + bookId + "\","
                                      + "\"SubscriberName\": \"" + subscriberName
                                      + "\"," + "\"Email\": \"" + email
                                      + "\"" + "}";
            //+ "}";
            return registerUserJSON;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriberName"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        private bool SubscriptionExists(string subscriberName, string bookId)
        {
            return _context.SubscriptionList.Any(e => e.SubscriberName.Equals(subscriberName) && e.BookId.Equals(bookId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpHead]
        [Route("healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("info")]
        public string Info()
        {
            return "Subscription Service - info";
        }
    }
}