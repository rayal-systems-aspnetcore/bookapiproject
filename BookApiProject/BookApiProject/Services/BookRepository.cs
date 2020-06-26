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

        public bool CreateBook(List<int> authorsId, List<int> categoriesId, Book book) {
            var authors = _bookContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();
            var categories = _bookContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            foreach (var author in authors) {
                var bookAuthor = new BookAuthor() { 
                    Author = author,
                    Book = book
                };
                _bookContext.Add(bookAuthor);
            }

            foreach (var category in categories) {
                var bookCategory = new BookCategory() {
                    Category = category,
                    Book = book
                };
                _bookContext.Add(bookCategory);
            }

            _bookContext.Add(book);

            return Save();
        }

        public bool DeleteBook(Book book) {
            _bookContext.Remove(book);
            return Save();
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

        public bool Save() {
            var saved = _bookContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateBook(List<int> authorsId, List<int> categoriesId, Book book) {
            var authors = _bookContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();
            var categories = _bookContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            var bookAuthorsToDelete = _bookContext.BookAuthors.Where(b => b.BookId == book.Id);
            var bookCategoriesToDelete = _bookContext.BookCategories.Where(c => c.CategoryId == book.Id);

            _bookContext.RemoveRange(bookAuthorsToDelete);
            _bookContext.RemoveRange(bookCategoriesToDelete);

            foreach (var author in authors) {
                var bookAuthor = new BookAuthor() {
                    Author = author,
                    Book = book
                };
                _bookContext.Add(bookAuthor);
            }

            foreach (var category in categories) {
                var bookCategory = new BookCategory() {
                    Category = category,
                    Book = book
                };
                _bookContext.Add(bookCategory);
            }

            _bookContext.Update(book);

            return Save();
        }
    }
}