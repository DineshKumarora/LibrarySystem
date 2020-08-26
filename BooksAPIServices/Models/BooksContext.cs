using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BooksAPIServices.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BooksContext : DbContext
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public BooksContext(DbContextOptions<BooksContext> options)
            : base(options)
        {
            LoadBooks();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadBooks()
        {

            if (BookLists?.ToArray().Length > 0)
                return;

            BookLists.Add(new Books
            {
                id = "B1212",
                bookName = "History of Amazon Valley",
                author = "Ross Suarez",
                copiesAvailable = 0,
                totalCopies = 2
            });
            BookLists.Add(new Books
            {
                id = "B4232",
                bookName = "Language Fundamentals",
                author = "H S Parkmay",
                copiesAvailable = 5,
                totalCopies = 5
            });
            SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Books> BookLists { get; set; }

    }

}
