using HomeNetAPI.Models;
using System.Collections.Generic;

namespace HomeNetAPI.Repository
{
    public interface ICountryRepository
    {
        List<Country> GetCountries();
        Country AddCountry(Country newCountry);
        Country DeleteCountry(int countryId);
        Country GetCountry(int countryId);
    }
}
