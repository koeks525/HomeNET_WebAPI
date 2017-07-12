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

        public List<HousePostFlag> GetFlaggedPosts(int houseID)
        {
            var houseMembers = dbContext.HouseMembers.Where(i => i.HouseID == houseID).ToList();
            List<HousePostFlag> housePostList = new List<HousePostFlag>();
            if (houseMembers != null)
            {
                foreach (HouseMember member in houseMembers)
                {
                    var housePosts = dbContext.FlaggedHousePosts.Where(i => member.HouseMemberID == member.HouseMemberID && i.IsFlagged == 1).ToList();
                    if (housePosts != null)
                    {
                        foreach (HousePostFlag flaggedPost in housePosts)
                        {
                            housePostList.Add(flaggedPost);
                        }
                    }
                }

                return housePostList;
            } else
            {
                return null;
            }
            
            
        }

        public List<HousePostFlag> GetPendingPosts(int houseID)
        {
            var houseMembers = dbContext.HouseMembers.Where(i => i.HouseID == houseID).ToList();
            List<HousePostFlag> flaggedPostList = new List<HousePostFlag>();
            if (houseMembers !=null) {
                foreach (HouseMember member in houseMembers)
                {
                    var postList = dbContext.FlaggedHousePosts.Where(i => i.HouseMemberID == member.HouseMemberID && i.IsFlagged == 0).ToList();
                    if (postList != null)
                    {
                        foreach (HousePostFlag post in postList)
                        {
                            flaggedPostList.Add(post);
                        }
                    }
                }
                if (flaggedPostList.Count > 0)
                {
                    return flaggedPostList;
                } else
                {
                    return null;
                }


            } else
            {
                return null;
            }
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
