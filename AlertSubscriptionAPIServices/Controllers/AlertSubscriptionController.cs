using System;
using AlertSubscriptionService.Models;
using AlertSubscriptionService.RabbitMQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlertSubscriptionService.Controllers
{
    /// <summary>
    /// Alert Subscription Controller
    /// </summary>
    [Route("api/[controller]")]
    public class AlertSubscriptionController : Controller
    {
        /// <summary>
        /// Alert Subscription
        /// </summary>
        /// <param name="subscriber">Subscriber</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult AlertSubscription([FromBody]Subscription subscriber)
        {
            try
            {
                RabbitMQClient client = new RabbitMQClient(subscriber);
                client.AddAlert(subscriber);
                client.Close();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(subscriber);
        }


        /// <summary>
        /// Send Subscription
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{bookId}")]
        public ActionResult SendSubscription(string bookId)
        {
            try
            {
                RabbitMQClient client = new RabbitMQClient(new Subscription { BookId = bookId });
                client.SendAlert();
                client.Close();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// Service Health Check
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
        /// Service Info
        /// </summary>
        /// <returns></returns>
        [HttpGet("info")]
        public string Info()
        {
            return "Alert Subscription Service - info";
        }
    }
}

