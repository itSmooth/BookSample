using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookSample.Data;
using BookSample.Models;

namespace BookSample.Controllers
{
    [Route("books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookSampleContext _context;

        public BooksController(BookSampleContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBook()
        {
            return await _context.Book.ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        [Route("{id:int}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Book.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpGet]
        [Route("{genre}")]
        public IActionResult GetBookByGenre(string genre)
        {
            var books = _context.Book.Include(b => b.Author)
                                .Where(b => b.Genre.Equals(genre));

            return Ok(books);
        }

        [HttpGet]
        [Route("~/authors/{authorId:int}/books")]
        public IActionResult GetBooksByAuthor(int authorId)
        {
            var author = _context.Book
                                .Include(b => b.Author)
                                .Where(b => b.AuthorId == authorId);

            return Ok(author);
        }

        [HttpGet]
        [Route("date/{**publishdate:datetime}")]
        public IActionResult Get(DateTime publishdate)
        {
            var books = _context.Book
                .Include(b => b.Author)
                .Where(b => b.PublishDate == publishdate);

            return Ok(books);
        }


        // PUT: api/Books/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Book.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.BookId }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();

            return book;
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.BookId == id);
        }
    }
}
