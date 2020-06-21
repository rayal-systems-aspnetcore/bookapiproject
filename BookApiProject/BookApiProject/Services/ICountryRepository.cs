using BookApiProject.Models;
using System.Collections.Generic;

namespace BookApiProject.Services {
    public interface ICountryRepository {
        ICollection<Country> GetCountries();
        Country GetCountry(int countryId);
        Country GetCountryOfAnAuthor(int authorId);
        ICollection<Author> GetAuthorsFromACountry(int countryId);
        bool CountryExists(int countryId);
    }
}
