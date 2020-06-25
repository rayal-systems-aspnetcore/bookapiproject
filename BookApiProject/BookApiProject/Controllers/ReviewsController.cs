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
    public class ReviewsController : Controller {
        private IReviewRepository _reviewRepo;
        private IReviewerRepository _reviewerRepo;
        private IBookRepository _bookRepo;
        public ReviewsController(IReviewerRepository reviewerRepo, IReviewRepository reviewRepo,
            IBookRepository bookRepo) {
            _reviewerRepo = reviewerRepo;
            _reviewRepo = reviewRepo;
            _bookRepo = bookRepo;
        }

        // api/reviews
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewers() {
            var reviews = _reviewRepo.GetReviews();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();

            foreach (var review in reviews) {
                reviewsDto.Add(new ReviewDto { 
                    Id = review.Id,
                    Headline = review.Headline,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText
                });
            }

            return Ok(reviewsDto);
        }

        //api/reviews/reviewId
        [HttpGet("{reviewId}", Name = "GetReview")]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReview(int reviewId) {
            if (!_reviewRepo.ReviewExists(reviewId))
                return NotFound();

            var review = _reviewRepo.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewDto = new ReviewDto() {
                Id = review.Id,
                Headline = review.Headline,
                Rating = review.Rating,
                ReviewText = review.ReviewText
            };
            return Ok(reviewDto);
        }

        // api/reviews/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewsForABook(int bookId) {
            if (!_bookRepo.BookExists(bookId))
                return NotFound();

            var reviews = _reviewRepo.GetReviewsOfABook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviews) {
                reviewsDto.Add(new ReviewDto() {
                    Id = review.Id,
                    Headline = review.Headline,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText
                });
            }
            return Ok(reviewsDto);
        }

        // api/reviews/reviewId/book
        [HttpGet("{reviewId}/book")]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetBookOfAReview(int reviewId) {
            
            if (!_reviewRepo.ReviewExists(reviewId))
                return NotFound();

            var book = _reviewRepo.GetBookOfAReview(reviewId);

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

        // api/review
        [HttpPost()]
        [ProducesResponseType(201, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateReview([FromBody] Review reviewToCreate) {

            if (reviewToCreate == null)
                return BadRequest(ModelState);

            if (!_reviewRepo.ReviewExists(reviewToCreate.Reviewer.Id))
                ModelState.AddModelError("", "Reviewer doesn't exists!");

            if (!_bookRepo.BookExists(reviewToCreate.Book.Id))
                ModelState.AddModelError("", "Book doesn't exists!");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            reviewToCreate.Book = _bookRepo.GetBook(reviewToCreate.Book.Id);
            reviewToCreate.Reviewer = _reviewerRepo.GetReviewer(reviewToCreate.Reviewer.Id);

            if (!_reviewRepo.CreateReview(reviewToCreate)) {
                ModelState.AddModelError("", $"Something went wrong saving review");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetReview", new { reviewId = reviewToCreate.Id }, reviewToCreate);
        }

        // api/reviews/reviewId
        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReview(int reviewId, [FromBody] Review reviewToUpdate) {

            if (reviewToUpdate == null)
                return BadRequest(ModelState);

            if (reviewId != reviewToUpdate.Id)
                return BadRequest(ModelState);

            if (!_reviewRepo.ReviewExists(reviewId))
                ModelState.AddModelError("", "Review doesn't exists");

            if (!_reviewRepo.ReviewExists(reviewToUpdate.Reviewer.Id))
                ModelState.AddModelError("", "Reviewer doesn't exists!");

            if (!_bookRepo.BookExists(reviewToUpdate.Book.Id))
                ModelState.AddModelError("", "Book doesn't exists!");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            reviewToUpdate.Book = _bookRepo.GetBook(reviewToUpdate.Book.Id);
            reviewToUpdate.Reviewer = _reviewerRepo.GetReviewer(reviewToUpdate.Reviewer.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepo.UpdateReview(reviewToUpdate)) {
                ModelState.AddModelError("", $"Something went wrong updating review");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        // api/reviews/reviewId
        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)] // No content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReview(int reviewId) {
            if (!_reviewRepo.ReviewExists(reviewId))
                return NotFound();

            var reviewToDelete = _reviewRepo.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepo.DeleteReview(reviewToDelete)) {
                ModelState.AddModelError("", $"Something went wrong deleting review.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }



    }
}
