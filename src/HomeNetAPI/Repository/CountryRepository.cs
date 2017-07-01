using System;
using HomeNetAPI.Models;
using System.Linq;
using System.Collections.Generic;

namespace HomeNetAPI.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private HomeNetContext homeNetContext;

        public CountryRepository(HomeNetContext context)
        {
            homeNetContext = context;
        }

        public Country AddCountry(Country newCountry)
        {
            var country = homeNetContext.Countries.Add(newCountry);
            homeNetContext.SaveChanges();
            newCountry.CountryID = country.Entity.CountryID;
            return newCountry;
        }

        public Country DeleteCountry(int countryId)
        {
            var countryToDelete = homeNetContext.Countries.FirstOrDefault(c => c.CountryID == countryId);
            countryToDelete.IsDeleted = 1;
            homeNetContext.SaveChanges();
            return countryToDelete;
        }

        public List<Country> GetCountries()
        {
            return homeNetContext.Countries.ToList();
        }

        public Country GetCountry(int countryId)
        {
            return homeNetContext.Countries.FirstOrDefault(c => c.CountryID == countryId);
        }
    }
}
