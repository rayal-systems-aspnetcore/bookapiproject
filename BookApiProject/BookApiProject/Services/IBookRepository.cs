using System.Collections.Generic;
using BookApiProject.Models;

namespace BookApiProject.Services {
    public interface IBookRepository {
        ICollection<Book> GetBooks();
        Book GetBook(int bookId);
        Book GetBook(string bookISBN);
        decimal GetBookRating(int bookId);
        bool BookExists(int bookId);
        bool BookExists(string bookISBN);
        bool IsDuplicateISBN(int bookId, string bookISBN);

        bool CreateBook(List<int> authorsId, List<int> categoriesId, Book book);
        bool UpdateBook(List<int> authorsId, List<int> categoriesId, Book book);
        bool DeleteBook(Book book);
        bool Save();
    }
}