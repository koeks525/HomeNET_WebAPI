using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private HomeNetContext dbContext;

        public CommentRepository(HomeNetContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public HousePostComment AddComment(HousePostComment newComment)
        {
            var result = dbContext.HousePostComments.Add(newComment);
            dbContext.SaveChanges();
            return result.Entity;
        }

        public HousePostComment DeleteComment(int housePostCommentID)
        {
            var result = dbContext.HousePostComments.First(i => i.HousePostCommentID == housePostCommentID);
            if (result == null)
            {
                return null;
            } else
            {
                dbContext.HousePostComments.Remove(result);
                dbContext.SaveChanges();
                return result;
            }
        }

        public HousePostComment GetComment(int housePostCommentID)
        {
            var result = dbContext.HousePostComments.First(i => i.HousePostCommentID == housePostCommentID);
            return result;
        }

        public List<HousePostComment> GetComments(int housePostID)
        {
            var result = dbContext.HousePostComments.Where(i => i.HousePostID == housePostID).ToList();
            return result;
        }
    }
}
