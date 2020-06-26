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
        private IAuthorRepository _authorRepo;
        private ICategoryRepository _cateRepo;
        private IReviewRepository _reviewRepo;
        public BooksController(IBookRepository bookRepo, IAuthorRepository authorRepo, 
            ICategoryRepository catRepo, IReviewRepository reviewRepo) {
            _bookRepo = bookRepo;
            _authorRepo = authorRepo;
            _cateRepo = catRepo;
            _reviewRepo = reviewRepo;
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
        [HttpGet("{bookId}", Name = "GetBook")]
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

        // api/books?authId=1&authId=2&cat=1&catId=2
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Book))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateBook([FromQuery] List<int> authId, [FromQuery] List<int> catId,
            [FromBody] Book bookToCreate) {

            var statusCode = ValidateBook(authId, catId, bookToCreate);

            if (!ModelState.IsValid)
                return BadRequest(statusCode.StatusCode);

            if (!_bookRepo.CreateBook(authId, catId, bookToCreate)) {
                ModelState.AddModelError("", $"Someting went wrong creating the book ${bookToCreate.Title}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetBook", new { bookId = bookToCreate.Id }, bookToCreate);
        }


        // api/books/bookId?authId=1&authId=2&cat=1&catId=2
        [HttpPut("{bookId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateBook(int bookId, [FromQuery] List<int> authId, [FromQuery] List<int> catId,
            [FromBody] Book bookToUpdate) {

            var statusCode = ValidateBook(authId, catId, bookToUpdate);

            if (bookId != bookToUpdate.Id)
                return BadRequest(statusCode.StatusCode);

            if (!_bookRepo.BookExists(bookId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(statusCode.StatusCode);

            if (!_bookRepo.UpdateBook(authId, catId, bookToUpdate)) {
                ModelState.AddModelError("", $"Someting went wrong updating the book ${bookToUpdate.Title}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        // api/reviewers/reviewerId
        [HttpDelete("{bookId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public IActionResult DeleteBook(int bookId) {
            if (!_bookRepo.BookExists(bookId))
                return NotFound();

            var reviewsToDelete = _reviewRepo.GetReviewsOfABook(bookId);
            var bookToDelete = _bookRepo.GetBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_reviewRepo.DeleteReviews(reviewsToDelete.ToList())) {
                ModelState.AddModelError("", $"Something went wrong deleting reviews");
                return StatusCode(500, ModelState);
            }

            if (!_bookRepo.DeleteBook(bookToDelete)) {
                ModelState.AddModelError("", $"Something went wrong deleting book ${bookToDelete.Title}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        private StatusCodeResult ValidateBook(List<int> authId, List<int> catId, Book book) {
            if (book == null || authId.Count() <= 0 || catId.Count() <= 0) {
                ModelState.AddModelError("", "Missing book, author, or category");
                return BadRequest();
            }

            if (_bookRepo.IsDuplicateISBN(book.Id, book.ISBN)) {
                ModelState.AddModelError("", "Duplicate ISBN");
                return StatusCode(422);
            }

            foreach (var id in authId) {
                if (!_authorRepo.AuthorExists(id)) {
                    ModelState.AddModelError("", "Author not found");
                    return StatusCode(404);
                }
            }

            foreach (var id in catId) {
                if (!_cateRepo.CategoryExists(id)) {
                    ModelState.AddModelError("", "Category not found");
                    return StatusCode(404);
                }
            }

            if (!ModelState.IsValid) {
                ModelState.AddModelError("", "Critical Error");
                return BadRequest();
            }

            return NoContent();
        }
    }
}