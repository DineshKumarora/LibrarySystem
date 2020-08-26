using System.Threading;
using System.Threading.Tasks;
using BooksAPIServices.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace BooksAPIServices.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly BooksContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public BooksController(BooksContext context)
        {
            _context = context;
        }

        //GET: api/Books
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Books>>> GetBookList()
        {
            return await _context.BookLists.ToListAsync();
        }

        //GET: api/Books/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Books>> GetBook(string id)
        {
            var books = await _context.BookLists.FindAsync(id);

            if (books == null)
                return NotFound();

            return Ok(books);
        }

        // PUT: api/Books/5
        // To protect from over posting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<Books>> UpdateAvailability(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var book = _context.BookLists.Find(id);
            if (book == null)
                return BadRequest();

            if (book.copiesAvailable > book.totalCopies || book.copiesAvailable <= 0)
                return BadRequest();

            try
            {
                if (book.copiesAvailable > 0)
                {
                    book.copiesAvailable -= 1;
                    _context.Entry(book).State = EntityState.Modified;
                    //_context.Entry(books).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return UnprocessableEntity(book);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BooksExists(id))
                    return NotFound();
                throw;
            }

            return Ok(book);
        }

        private bool BooksExists(string id)
        {
            return _context.BookLists.Any(e => e.id.Equals(id));
        }

        //// POST: api/Books
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="books"></param>
        ///// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //public async Task<ActionResult<Books>> PostBooks(Books books)
        //{
        //    _context.BookLists.Add(books);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetBookList), new { id = books.id }, books);
        //}
        
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
            return "Book Service - info";
        }
    }
}

