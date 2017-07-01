using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class HouseImageRepository : IHouseImageRepository
    {
        private HomeNetContext homeNetContext;
        public HouseImageRepository(HomeNetContext homeNetContext)
        {
            this.homeNetContext = homeNetContext;
        }

        public HouseProfileImage AddHouseProfileImage(HouseProfileImage Image)
        {
            var result = homeNetContext.HouseProfileImages.Add(Image);
            homeNetContext.SaveChanges();
            return result.Entity;
        }

        public HouseProfileImage DeleteHouseProfileImage(HouseProfileImage Image)
        {
            throw new NotImplementedException();
        }
    }
}
