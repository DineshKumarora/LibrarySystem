namespace SubscriptionAPIService.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Books
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string bookName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long copiesAvailable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long totalCopies { get; set; }
    }
}
