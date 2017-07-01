using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface IHouseImageRepository
    {
        HouseProfileImage AddHouseProfileImage(HouseProfileImage Image);
        HouseProfileImage DeleteHouseProfileImage(HouseProfileImage Image);
    }
}
