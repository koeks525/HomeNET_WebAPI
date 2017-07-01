using HomeNetAPI.Models;
using System.Collections.Generic;
namespace HomeNetAPI.Repository
{
    public interface IHouseRepository
    {
        House CreateHouse(House newHouse);
        House GetHouse(int houseID);
        House DeleteHouse(int houseID);
        House UpdateHouse(House updateHouse);
        List<House> GetHouses(int userID);
        List<House> SearchHouses(string searchParameters);
        

    }
}
