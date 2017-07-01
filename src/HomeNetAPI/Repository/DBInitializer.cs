using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public static class DBInitializer
    {
        public static void Initialize(HomeNetContext context)
        {
            context.Database.EnsureCreated();
            var fileLines = System.IO.File.ReadAllLines("wwwroot/updated_list_of_countries.txt");
            List<Country> countries = new List<Country>();
            foreach (String line in fileLines)
            {
                Country currentCountry = new Country()
                {
                    CountryID = 0,
                    Name = line,
                    IsDeleted = 0

                };
                countries.Add(currentCountry);
            }
            context.Countries.AddRange(countries);
            //Add the needed keys to the system
            Key facebookKey = new Key()
            {
                KeyID = 0,
                Name = "facebook_app_id",
                Description = "Facebook API Key for Android",
                Value = "174137489749832",
                IsDeleted = 0
            };
            
            Key twitterKey = new Key()
            {
                KeyID = 0,
                Name = "twitter_api_key",
                Description = "Twitter API Key For Android",
                Value = "t93jnVGNgYHRL1hL4cAZG7hUX",
                IsDeleted = 0
            };

            Key twitterSecret = new Key()
            {
                KeyID = 0,
                Name = "twitter_api_secret",
                Description = "Twitter API Secret for Android",
                Value = "7jB0HTdVlvQRwX1WzwhJAzs6U3rGe3aKJ0PlLq7GbKiT38Sy2l"
            };

            context.Keys.AddRange(facebookKey, twitterKey, twitterSecret);
            context.SaveChanges();
        }
    }
}
