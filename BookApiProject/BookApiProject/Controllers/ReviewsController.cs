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
        [HttpGet("{reviewId}")]
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




    }
}
