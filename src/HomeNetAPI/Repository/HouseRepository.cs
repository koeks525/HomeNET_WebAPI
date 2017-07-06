using System;
using System.Collections.Generic;
using HomeNetAPI.Models;
using System.Linq;

namespace HomeNetAPI.Repository
{
    public class HouseRepository : IHouseRepository
    {
        private HomeNetContext homeNetContext;
        public HouseRepository(HomeNetContext context)
        {
            homeNetContext = context;
        }

        public House CreateHouse(House newHouse)
        {
            
            var house = homeNetContext.Houses.Add(newHouse);
            homeNetContext.SaveChanges();
            return house.Entity;
        }

        public House DeleteHouse(int houseID)
        {
            var house = homeNetContext.Houses.FirstOrDefault(h => h.HouseID == houseID);
            house.IsDeleted = 1;
            homeNetContext.SaveChanges();
            return house;
        }

        public House GetHouse(int houseID)
        {
            var house = homeNetContext.Houses.FirstOrDefault(h => h.HouseID == houseID);
            return house;
        }

        public House UpdateHouse(House updateHouse)
        {
            var house = new House()
            {
                HouseID = updateHouse.HouseID,
                Description = updateHouse.Description,
                Location = updateHouse.Location,
                DateCreated = updateHouse.DateCreated,
                HouseImage = updateHouse.HouseImage,
                IsDeleted = updateHouse.IsDeleted,
                Name = updateHouse.Name,
                UserID = updateHouse.UserID

            };
            var result = homeNetContext.Houses.FirstOrDefault(h => h.HouseID == house.HouseID);
            if (result == null)
            {
                return null;
            } else
            {
                result = house;
                homeNetContext.SaveChanges();
                return result;
            }
            
        }

        public List<House> GetHouses(int userID)
        {
            var resultHouses = homeNetContext.Houses.Where(u => u.UserID == userID && u.IsDeleted == 0).ToList();
            return resultHouses;
        }

        public List<House> SearchHouses(String searchParameter)
        {
            var list = homeNetContext.Houses.Where(i => i.Description.ToLower().Contains(searchParameter.ToLower()) || i.Name.ToLower().Contains(searchParameter.ToLower())).ToList();
            return list;

        }

    }
}
