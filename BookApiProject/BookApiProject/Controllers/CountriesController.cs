using BookApiProject.Dtos;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BookApiProject.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : Controller {
        private ICountryRepository _countryRepository;
        private IAuthorRepository _authorRepo;
        
        public CountriesController(ICountryRepository repository, IAuthorRepository authorRepo) {
            _countryRepository = repository;
            _authorRepo = authorRepo;
        }

        // api/countries
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries() {
            var countries = _countryRepository.GetCountries().ToList();

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var countriesDto = new List<CountryDto>();

            foreach (var country in countries) {
                countriesDto.Add(new CountryDto { 
                    Id = country.Id,
                    Name = country.Name
                });
            }

            return Ok(countriesDto);
        }

        //api/countries/countryId
        [HttpGet("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountry(int countryId) {

            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var country = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var countryDto = new CountryDto() { 
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }

        //api/countries/authors/authorId
        [HttpGet("authors/{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountryOfAnAuthor(int authorId) {
            if (!_authorRepo.AuthorExists(authorId))
                return NotFound();

            var country = _countryRepository.GetCountryOfAnAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest();

            var countryDto = new CountryDto() { 
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }

        // api/countries/countryId/authors
        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(200, Type =  typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAuthorsFromACountry(int countryId) {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var authors = _countryRepository.GetAuthorsFromACountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorDto = new List<AuthorDto>();

            foreach (var author in authors) {
                authorDto.Add(new AuthorDto() { 
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }
            return Ok(authorDto);
        }
    }
}
