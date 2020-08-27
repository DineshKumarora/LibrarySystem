using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SubscriptionAPIService.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionContext : DbContext
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public SubscriptionContext(DbContextOptions<SubscriptionContext> options)
            : base(options)
        {
            LoadSubscription();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadSubscription()
        {

            if (SubscriptionList?.ToArray().Length > 0)
                return;

            if (SubscriptionList != null)
            {
                SubscriptionList.Add(new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriberName = "John",
                    BookId = "B1212",
                    SubscriptionDate = "12-JUN-2020",
                    ReturnedDate = "",
                });
                SubscriptionList.Add(new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriberName = "Mark",
                    BookId = "B4232",
                    SubscriptionDate = "26-APR-2020",
                    ReturnedDate = "14-May-2020",
                });
                SubscriptionList.Add(new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    SubscriberName = "Peter",
                    BookId = "B1212",
                    SubscriptionDate = "22-JUN-2020",
                    ReturnedDate = "",
                });
            }

            SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Subscription> SubscriptionList { get; set; }

    }

}
