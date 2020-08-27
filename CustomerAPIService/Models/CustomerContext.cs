using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CustomerAPIService.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerContext : DbContext
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {
            LoadCustomer();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadCustomer()
        {

            if (CustomerList?.ToArray().Length > 0)
                return;

            if (CustomerList != null)
            {
                CustomerList.Add(new Customer
                {
                    Id = "John",
                    SubscriberName = "John Millar",
                    Password = "ASDJAS1@21SDDEEEE",
                    Email = "abc@xyz.com",
                });
                CustomerList.Add(new Customer
                {
                    Id = "Mark",
                    SubscriberName = "Mark Waugh",
                    Password = "ASDJAS12#231SDDEEE",
                    Email = "abc2@pqr.com",
                });
                CustomerList.Add(new Customer
                {
                    Id = "Peter",
                    SubscriberName = "Peter Parker",
                    Password = "ASDJ1231A@S121SDD",
                    Email = "abc3@lmn.com",
                });
                CustomerList.Add(new Customer
                {
                    Id = "Dinesh",
                    SubscriberName = "Dinesh Arora",
                    Password = "Dinesh",
                    Email = "Dinesh@lmn.com",
                });
            }

            SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Customer> CustomerList { get; set; }

    }

}
