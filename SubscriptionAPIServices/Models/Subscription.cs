namespace SubscriptionAPIService.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SubscriberName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SubscriptionDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReturnedDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BookId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Notify { get; set; }
    }
}
