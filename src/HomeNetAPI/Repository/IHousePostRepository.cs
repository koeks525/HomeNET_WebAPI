using HomeNetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.Repository
{
    public interface IHousePostRepository
    {
        List<HousePost> GetHousePosts(int houseID);
        HousePost AddHousePost(HousePost newPost);
        HousePost DeleteHousePost(int housePostID);
        HousePost FlagHousePost(HousePostFlag flaggedPost);
        HousePost GetHousePost(int housePostID);
        HousePost UpdateHousePost(HousePost selectedHousePost);
    }
}
