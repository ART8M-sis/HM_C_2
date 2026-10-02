using D_ASP_2.Models;
using Microsoft.AspNetCore.Mvc;

namespace D_ASP_2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private static readonly List<Book> _books = new()
        {
            new Book { Id = 1, Title = "Кобзар",               Author = "Тарас Шевченко",        Year = 1840 },
            new Book { Id = 2, Title = "Місто",                Author = "Валер'ян Підмогильний", Year = 1928 },
            new Book { Id = 3, Title = "Тіні забутих предків", Author = "Михайло Коцюбинський",  Year = 1911 }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_books);
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string? title, [FromQuery] string? author)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Query parameter 'title' is required.");

            var query = _books.Where(b =>
                b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(author))
                query = query.Where(b =>
                    b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));

            return Ok(query.ToList());
        }

        [HttpPost]
        public IActionResult Create([FromBody] Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title) ||
                string.IsNullOrWhiteSpace(book.Author))
                return BadRequest("Title and Author are required.");

            if (book.Year < 1800)
                return BadRequest("Year must be 1800 or later.");

            book.Id = _books.Any() ? _books.Max(b => b.Id) + 1 : 1;
            _books.Add(book);

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Book book)
        {
            var existing = _books.FirstOrDefault(b => b.Id == id);
            if (existing is null)
                return NotFound("Book not found");

            if (string.IsNullOrWhiteSpace(book.Title) ||
                string.IsNullOrWhiteSpace(book.Author))
                return BadRequest("Title and Author are required.");

            if (book.Year < 1800)
                return BadRequest("Year must be 1800 or later.");

            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.Year = book.Year;

            return Ok(existing);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound("Book not found");

            _books.Remove(book);
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound("Book not found");

            return Ok(book);
        }
    }
}