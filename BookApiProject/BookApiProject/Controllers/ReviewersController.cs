using BookApiProject.Dtos;
using BookApiProject.Models;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.WindowsRuntime;

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
        [HttpGet("{reviewerId}", Name = "GetReviewer")]
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
                    Id = review.Id,
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

        // api/reviewers
        [HttpPost]
        [ProducesResponseType(201, Type = (typeof(Reviewer)))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateReviewer([FromBody] Reviewer reviewerToCreate) {

            if (reviewerToCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_repo.CreateReviewer(reviewerToCreate)) {
                ModelState.AddModelError("", $"Something went wrong deleting {reviewerToCreate.FirstName} {reviewerToCreate.LastName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetReviewer", new { reviewerId = reviewerToCreate.Id }, reviewerToCreate);
        }

        //api/reviewers/reviewerId
        [HttpPut("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] Reviewer reviewerToUpdate) {
            if (reviewerToUpdate == null)
                return BadRequest(ModelState);

            if (reviewerId != reviewerToUpdate.Id)
                return BadRequest(ModelState);

            if (!_repo.ReviewerExists(reviewerId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_repo.UpdateReviewer(reviewerToUpdate)) {
                ModelState.AddModelError("", $"Something went wrong updating ${reviewerToUpdate.FirstName} {reviewerToUpdate.LastName}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        // api/reviewers/reviewerId
        [HttpDelete("{reviewerId}")]
        public IActionResult DeleteReviewer(int reviewerId) {
            if (!_repo.ReviewerExists(reviewerId))
                return NotFound();

            var reviewerToDelete = _repo.GetReviewer(reviewerId);

            var reviewsToDelete = _repo.GetReviewsByReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_repo.DeleteReviewer(reviewerToDelete)) {
                ModelState.AddModelError("", $"Something went wrong deleting ${reviewerToDelete.FirstName} {reviewerToDelete.LastName}");
                return StatusCode(500, ModelState);
            }

            if (!_reviewRepo.DeleteReviews(reviewsToDelete.ToList())) {
                ModelState.AddModelError("", $"Something went wrong deleting reviews by ${reviewerToDelete.FirstName} {reviewerToDelete.LastName}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}