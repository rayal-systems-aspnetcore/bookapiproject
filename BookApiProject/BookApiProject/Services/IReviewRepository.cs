using System.Collections.Generic;
using BookApiProject.Models;

namespace BookApiProject.Services {
    public interface IReviewRepository {
        ICollection<Review> GetReviews();
        Review GetReview(int reviewId);
        ICollection<Review> GetReviewsOfABook(int bookId);
        Book GetBookOfAReview(int reviewId);
        bool ReviewExists(int reviewId);
    }
}