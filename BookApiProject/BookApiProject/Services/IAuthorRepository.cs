using System.Collections.Generic;
using BookApiProject.Models;

namespace BookApiProject.Services {
    public interface IAuthorRepository {
        ICollection<Author> GetAuthors();
        Author GetAuthor(int authorId);
        ICollection<Author> GetAuthorsOfABook(int bookId);
        ICollection<Book> GetBooksByAuthor(int authorId);
        bool AuthorExists(int authorId);
    }
}
