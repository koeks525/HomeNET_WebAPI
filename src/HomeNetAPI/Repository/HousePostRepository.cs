﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class HousePostRepository : IHousePostRepository
    {
        private HomeNetContext homeNetContext;

        public HousePostRepository(HomeNetContext homeNetContext)
        {
            this.homeNetContext = homeNetContext;
        }

        public HousePost AddHousePost(HousePost newPost)
        {
            var houseResult = homeNetContext.HousePosts.Add(newPost);
            homeNetContext.SaveChanges();
            return houseResult.Entity;
        }

        public HousePost DeleteHousePost(int housePostID)
        {
            var housePost = homeNetContext.HousePosts.First(i => i.HousePostID == housePostID);
            if (housePost != null)
            {
                housePost.IsDeleted = 1;
                homeNetContext.SaveChanges();
                return housePost;
            } else
            {
                return null;
            }
        }

        public HousePost FlagHousePost(HousePostFlag flaggedPost)
        {
            var housePost = homeNetContext.HousePosts.First(i => i.HousePostID == flaggedPost.HousePostID);
            if (housePost != null)
            {
                housePost.IsFlagged = 1;
                homeNetContext.FlaggedHousePosts.Add(flaggedPost);
                homeNetContext.SaveChanges();
                return housePost;
            }
            return null;
        }

        public List<HousePost> GetHousePosts(int houseID)
        {
            return homeNetContext.HousePosts.Where(item => item.IsDeleted != 1).ToList();
        }

        public HousePost GetHousePost(int housePostID)
        {
            return homeNetContext.HousePosts.First(i => i.HousePostID == housePostID);
        }
    }
}