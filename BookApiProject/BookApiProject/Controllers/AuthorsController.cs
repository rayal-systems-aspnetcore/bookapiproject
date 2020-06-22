using BookApiProject.Dtos;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : Controller {
        private IAuthorRepository _authorRepo;
        private IBookRepository _bookRepo;
        public AuthorsController(IAuthorRepository autherRepo, IBookRepository bookRepo) {
            _authorRepo = autherRepo;
            _bookRepo = bookRepo;
        }

        // api/authors
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetAuthors() {
            var authors = _authorRepo.GetAuthors();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();

            foreach (var author in authors) {
                authorsDto.Add(new AuthorDto { 
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }
            return Ok(authorsDto);
        }

        // api/authors/authorId
        [HttpGet("{authorId}")]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAuthor(int authorId) {
            if (!_authorRepo.AuthorExists(authorId))
                return NotFound();

            var author = _authorRepo.GetAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorDto = new AuthorDto() {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName
            };
            return Ok(authorDto);
        }

        // api/authors/authorId/books
        [HttpGet("{authorId}/books")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBookByAuthor(int authorId) {
            if (!_authorRepo.AuthorExists(authorId))
                return NotFound();

            var books = _authorRepo.GetBooksByAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();

            foreach (var book in books) {
                booksDto.Add(new BookDto() { 
                    Id = book.Id,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    DatePublished = book.DatePublished
                });
            }
            
            return Ok(booksDto);
        }


        // api/authors/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAuthorsOfABook(int bookId) {
            if (!_bookRepo.BookExists(bookId))
                return NotFound();

            var authors = _authorRepo.GetAuthorsOfABook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();

            foreach (var author in authors) {
                authorsDto.Add(new AuthorDto() { 
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }
            return Ok(authorsDto);
        }

        // GET
    }
}
