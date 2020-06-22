using BookApiProject.Dtos;
using BookApiProject.Models;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BookApiProject.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewersController : Controller {
        private IReviewerRepository _repo;
        private IReviewRepository _reviewRepo;
        public ReviewersController(IReviewerRepository repository, IReviewRepository reviewRepo) {
            _repo = repository;
            _reviewRepo = reviewRepo;
        }

        // api/reviewers
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewers() {
            var reviewers = _repo.GetReviewers();
            
            if (!ModelState.IsValid)
                return BadRequest();

            var reviewersDto = new List<ReviewerDto>();
            foreach (var reviewer in reviewers) {
                reviewersDto.Add(new ReviewerDto() {
                    Id = reviewer.Id,
                    FirstName = reviewer.FirstName,
                    LastName = reviewer.LastName
                });
            }
            return Ok(reviewersDto);
        }

        // api/reviewers/reviewerId
        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewers(int reviewerId) {
            
            if (!_repo.ReviewerExists(reviewerId))
                return NotFound();

            var reviewer = _repo.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto() { 
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };
            return Ok(reviewerDto);
        }

        // api/reviewers/reviewerId/reviews
        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewsByReviewer(int reviewerId) {
            
            if (!_repo.ReviewerExists(reviewerId))
                return NotFound();

            var reviews = _repo.GetReviewsByReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();

            foreach (var review in reviews) {
                reviewsDto.Add(new ReviewDto() {
                    Id= review.Id,
                    Headline = review.Headline,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText
                });
            }
            return Ok(reviewsDto);
        }

        // api/reviewers/reviewId/reviewer
        [HttpGet("{reviewId}/reviewer")]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewersOfAReview(int reviewId) {
            
            if (!_reviewRepo.ReviewExists(reviewId))
                return NotFound();

            var reviewer = _repo.GetReviewerOfAReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto() { 
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }

    }
}
