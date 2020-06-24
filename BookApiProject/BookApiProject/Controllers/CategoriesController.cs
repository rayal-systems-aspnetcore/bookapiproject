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
    public class CategoriesController : Controller {
        private ICategoryRepository _categoryRepo;
        private IBookRepository _bookRepository;
        public CategoriesController(ICategoryRepository repository, IBookRepository bookRepo) {
            _categoryRepo = repository;
            _bookRepository = bookRepo;
        }

        // api/categories
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategories() {
            var categories = _categoryRepo.GetCategories();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var categoriesDto = new List<CategoryDto>();

            foreach (var category in categories) {
                categoriesDto.Add(new CategoryDto { 
                    Id = category.Id,
                    Name = category.Name
                });
            }
            return Ok(categoriesDto);
        }

        // api/categories/categoryId
        [HttpGet("{categoryId}", Name = "GetCategory")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        public IActionResult GetCategory(int categoryId) {
            if (!_categoryRepo.CategoryExists(categoryId))
                return NotFound();

            var category = _categoryRepo.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest();

            var categoryDto = new CategoryDto() { 
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }

        // api/categories/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetAllCategoriesForABook(int bookId) {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var categories = _categoryRepo.GetAllCategoriesOfABook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoriesDto = new List<CategoryDto>();

            foreach(var category in categories) {
                categoriesDto.Add(new CategoryDto() { 
                    Id = category.Id,
                    Name = category.Name
                });
            }

            return Ok(categoriesDto);
        }

        // api/categories/categoryId/books
        [HttpGet("{categoryId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetAllBooksForCategory(int categoryId) {
            if (!_categoryRepo.CategoryExists(categoryId))
                return NotFound();

            var books = _categoryRepo.GetAllBooksForCategory(categoryId);

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

        // api/categories
        [HttpPost()]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategories([FromBody] Category categoryToCreate) {
            
            if (categoryToCreate == null)
                return BadRequest(ModelState);

            var category = _categoryRepo.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == categoryToCreate.Name.Trim().ToUpper())
                .FirstOrDefault();

            if(category != null) {
                ModelState.AddModelError("", $"Category {categoryToCreate.Name} already exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepo.CreateCategory(categoryToCreate)) {
                ModelState.AddModelError("", $"Something went wrong saving {categoryToCreate.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategory", new { categoryId = categoryToCreate.Id }, categoryToCreate);
        }

        // api/categories/categoryId
        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)] //no content so no type
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] Category updatedCategoryInfo) {

            if (updatedCategoryInfo == null)
                return BadRequest(ModelState);

            if (categoryId != updatedCategoryInfo.Id)
                return BadRequest(ModelState);

            if (!_categoryRepo.CategoryExists(categoryId))
                return NotFound();

            if (_categoryRepo.IsDuplicateCategoryName(categoryId, updatedCategoryInfo.Name)) {
                ModelState.AddModelError("", $"Category {updatedCategoryInfo.Name} already exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_categoryRepo.UpdateCategory(updatedCategoryInfo)) {
                ModelState.AddModelError("", $"Something went wrong saving {updatedCategoryInfo.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // api/categories/categoryId
        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)] //no content so no type
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCategory(int categoryId) {
            if (!_categoryRepo.CategoryExists(categoryId))
                return NotFound();

            var categoryToDelete = _categoryRepo.GetCategory(categoryId);

            if (_categoryRepo.GetAllBooksForCategory(categoryId).Count() > 0)
            {
                ModelState.AddModelError("", $"Category {categoryToDelete.Name} cannot be deleted because it is used by at least one book");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepo.DeleteCategory(categoryToDelete)) {
                ModelState.AddModelError("", $"Something went wrong deleting {categoryToDelete.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
