using BookApiProject.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services {
    public class BookRepository : IBookRepository {
        private BookDbContext _bookContext;
        public BookRepository(BookDbContext bookContext) {
            _bookContext = bookContext;
        }

        public bool BookExists(int bookId) {
            return _bookContext.Books.Any(b => b.Id == bookId);
        }

        public bool BookExists(string bookISBN) {
            return _bookContext.Books.Any(b => b.ISBN == bookISBN);
        }

        public Book GetBook(int bookId) {
            return _bookContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Book GetBook(string bookISBN) {
            return _bookContext.Books.Where(b => b.ISBN == bookISBN).FirstOrDefault();
        }

        public decimal GetBookRating(int bookId) {
            var reviews = _bookContext.Reviews.Where(r => r.Book.Id == bookId);

            if (reviews.Count() <= 0)
                return 0;

            return ((decimal)reviews.Sum(r => r.Rating) / reviews.Count());
        }

        public ICollection<Book> GetBooks() {
            return _bookContext.Books.OrderBy(b => b.Title).ToList();
        }

        public bool IsDuplicateISBN(int bookId, string bookISBN) {
            var book = _bookContext.Books.Where(b => b.ISBN.Trim().ToUpper() == bookISBN.Trim().ToUpper() && b.Id != bookId).FirstOrDefault();
            return book == null ? false : true;
        }
    }
}