using BookApiProject.Dtos;
using BookApiProject.Models;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller {
        private IBookRepository _bookRepo;
        public BooksController(IBookRepository bookRepo) {
            _bookRepo = bookRepo;
        }

        // api/books
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetBooks() {
            var books = _bookRepo.GetBooks();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();

            foreach (var book in books) {
                booksDto.Add(new BookDto() {
                    Id = book.Id,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    DatePublished = book.DatePublished
                });
            }
            return Ok(booksDto);
        }

        // api/books/bookId
        [HttpGet("{bookId}")]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBook(int bookId) {
            if (!_bookRepo.BookExists(bookId))
                return NotFound();

            var book = _bookRepo.GetBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto() { 
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                DatePublished = book.DatePublished
            };

            return Ok(bookDto);
        }

        // api/books/isbn/bookIsbn
        [HttpGet("isbn/{bookIsbn}")]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBook(string bookIsbn) {
            if (!_bookRepo.BookExists(bookIsbn))
                return NotFound();

            var book = _bookRepo.GetBook(bookIsbn);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto() {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                DatePublished = book.DatePublished
            };
            return Ok(bookDto);
        }
    }
}