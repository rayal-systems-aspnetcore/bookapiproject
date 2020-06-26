using BookApiProject.Dtos;
using BookApiProject.Models;
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
        private ICountryRepository _countryRepo;
        public AuthorsController(IAuthorRepository autherRepo, IBookRepository bookRepo,
            ICountryRepository countryRepo) {
            _authorRepo = autherRepo;
            _bookRepo = bookRepo;
            _countryRepo = countryRepo;
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
        [HttpGet("{authorId}", Name = "GetAuthor")]
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

        // api/authors
        [HttpPost()]
        [ProducesResponseType(201, Type = typeof(Author))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateAuthor([FromBody] Author authorToCreate) {

            if (authorToCreate == null)
                return BadRequest(ModelState);

            if (!_countryRepo.CountryExists(authorToCreate.Country.Id)) { 
                ModelState.AddModelError("", "Country doesn't exists!");
                return StatusCode(404, ModelState);
            }

            authorToCreate.Country = _countryRepo.GetCountry(authorToCreate.Country.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_authorRepo.CreateAuthor(authorToCreate)) {
                ModelState.AddModelError("", $"Something went wrong saving author ${authorToCreate.FirstName} ${authorToCreate.FirstName}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetAuthor", new { authorId = authorToCreate.Id }, authorToCreate);
        }

        // api/authors/authorId
        [HttpPut("{authorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateAuthor(int authorId, [FromBody] Author authorToUpdate) {

            if (authorToUpdate == null)
                return BadRequest(ModelState);

            if(authorId != authorToUpdate.Id) {
                return BadRequest(ModelState);
            }

            if (!_authorRepo.AuthorExists(authorId))
                ModelState.AddModelError("", "Author doesn't exists");

            if (!_countryRepo.CountryExists(authorToUpdate.Country.Id))
                ModelState.AddModelError("", "Country doesn't exists!");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            authorToUpdate.Country = _countryRepo.GetCountry(authorToUpdate.Country.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_authorRepo.UpdateAuthor(authorToUpdate)) {
                ModelState.AddModelError("", $"Something went wrong updating author ${authorToUpdate.FirstName} ${authorToUpdate.FirstName}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetAuthor", new { authorId = authorToUpdate.Id }, authorToUpdate);
        }

        // api/authors/authorId
        [HttpDelete("{authorId}")]
        [ProducesResponseType(204)] //no content so no type
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult DeleteAuthor(int authorId) {
            if (!_authorRepo.AuthorExists(authorId))
                return NotFound();

            var authorToDelete = _authorRepo.GetAuthor(authorId);

            if (_authorRepo.GetBooksByAuthor(authorId).Count() > 0) {
                ModelState.AddModelError("", $"Author {authorToDelete.FirstName} {authorToDelete.LastName} cannot be deleted because it is associated by at least one book");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_authorRepo.DeleteAuthor(authorToDelete)) {
                ModelState.AddModelError("", $"Something went wrong deleting {authorToDelete.FirstName} {authorToDelete.LastName}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
