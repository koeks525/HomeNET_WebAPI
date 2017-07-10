using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class FlaggedPostsRepository : IFlaggedPostRepository
    {
        private HomeNetContext dbContext;

        public FlaggedPostsRepository(HomeNetContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public HousePostFlag ApproveFlag(HousePostFlag newFlag)
        {
            var resultFlag = dbContext.FlaggedHousePosts.First(i => i.HousePostFlagID == newFlag.HousePostFlagID);
            if (resultFlag != null)
            {
                resultFlag.IsFlagged = 1;
                resultFlag.ResponseMessage = newFlag.ResponseMessage;
                resultFlag.DateFlagged = newFlag.DateFlagged;
                resultFlag.Message = newFlag.Message;
                dbContext.SaveChanges();
                return resultFlag;
            } else
            {
                return null;
            }

        }

        public HousePostFlag FlagHousePost(HousePostFlag newFlag)
        {
            var result = dbContext.FlaggedHousePosts.Add(newFlag);
            dbContext.SaveChanges();
            return result.Entity;
        }

        public List<HousePostFlag> GetFlaggedPosts(int housePostID)
        {
            var result = dbContext.FlaggedHousePosts.Where(i => i.HousePostID == housePostID).ToList();
            return result;
        }

        public HousePostFlag RemoveFlag(HousePostFlag newFlag)
        {
            var result = dbContext.FlaggedHousePosts.First(i => i.HousePostFlagID == newFlag.HousePostFlagID);
            if (result != null)
            {
                dbContext.FlaggedHousePosts.Remove(result);
                dbContext.SaveChanges();
                return result;
            } else
            {
                return null;
            }
        }
    }
}
