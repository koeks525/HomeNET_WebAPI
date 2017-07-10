using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface IFlaggedPostRepository
    {
        List<HousePostFlag> GetFlaggedPosts(int houseID);
        HousePostFlag FlagHousePost(HousePostFlag newFlag);
        HousePostFlag ApproveFlag(HousePostFlag newFlag);
        HousePostFlag RemoveFlag(HousePostFlag newFlag);
    }
}
