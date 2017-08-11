using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class HousePostMetaDataRepository : IHousePostMetaDataRepository
    {
        private HomeNetContext dbContext;

        public HousePostMetaDataRepository(HomeNetContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public List<HousePostMetaData> GetHousePostMetaData(int housePostID)
        {
            var dataList = dbContext.HousePostMetaData.Where(i => i.HousePostID == housePostID).ToList();
            return dataList;
        }

        public HousePostMetaData RegisterDislike(HousePostMetaData newData)
        {
            newData.Disliked = 1;
            var result = dbContext.HousePostMetaData.Add(newData); //Register 1
            dbContext.SaveChanges();
            return result.Entity;
        }

        public HousePostMetaData RegisterLike(HousePostMetaData newData)
        {
            newData.Liked = 1;
            var result = dbContext.HousePostMetaData.Add(newData);
            dbContext.SaveChanges();
            return result.Entity;
        }

        public HousePostMetaData UpdateMetaData(HousePostMetaData data)
        {
            var updateTask = dbContext.HousePostMetaData.First(i => i.HousePostMetaDataID == data.HousePostMetaDataID);
            updateTask = data;
            dbContext.SaveChanges();
            return updateTask;
        }
    }
}
