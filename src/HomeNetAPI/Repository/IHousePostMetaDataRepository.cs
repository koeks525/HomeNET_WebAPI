using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface IHousePostMetaDataRepository
    {
        HousePostMetaData RegisterLike(HousePostMetaData newData);
        HousePostMetaData RegisterDislike(HousePostMetaData newData);
        List<HousePostMetaData> GetHousePostMetaData(int housePostID);
        HousePostMetaData UpdateMetaData(HousePostMetaData data);
    }
}
