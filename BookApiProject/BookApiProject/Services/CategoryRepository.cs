using BookApiProject.Models;
using System.Collections.Generic;
using System.Linq;

namespace BookApiProject.Services {
    public class CategoryRepository : ICategoryRepository {
        private BookDbContext _categoryContext;
        public CategoryRepository(BookDbContext context) {
            _categoryContext = context;
        }
        public bool CategoryExists(int categoryId) {
            return _categoryContext.Categories.Any(c => c.Id == categoryId);
        }

        public ICollection<Book> GetAllBooksForCategory(int categoryId) {
            return _categoryContext.BookCategories.Where(c => c.CategoryId == categoryId)
                .Select(b => b.Book).ToList();
        }

        public ICollection<Category> GetCategories() {
            return _categoryContext.Categories.OrderBy(c => c.Name).ToList();
        }

        public ICollection<Category> GetAllCategoriesOfABook(int bookId) {
            return _categoryContext.BookCategories.Where(b => b.BookId == bookId).Select(c => c.Category).ToList();
        }

        public Category GetCategory(int categoryId) {
            return _categoryContext.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
        }
    }
}
