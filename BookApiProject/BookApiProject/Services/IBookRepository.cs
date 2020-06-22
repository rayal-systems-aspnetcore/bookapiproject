using System.Collections.Generic;
using BookApiProject.Models;

namespace BookApiProject.Services {
    public interface IBookRepository {
        ICollection<Book> GetBooks();
        Book GetBook(int bookId);
        bool GetBook(string bookISBN);
        decimal GetBookRating(int bookId);
        bool BookExists(int bookId);
        Book BookExists(string bookISBN);
        bool IsDuplicateISBN(int bookId, string bookISBN);
    }
}