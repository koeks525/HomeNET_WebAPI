using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class AnnouncementCommentRepository : IAnnouncementCommentRepository
    {
        private HomeNetContext dbContext;

        public AnnouncementCommentRepository(HomeNetContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public AnnouncementComment AddAnnouncementComment(AnnouncementComment newOne)
        {
            var result = dbContext.AnnouncementComments.Add(newOne);
            dbContext.SaveChanges();
            return result.Entity;
        }

        public AnnouncementComment FlagAnnouncementComment(int announcementCommentID)
        {
            var comment = dbContext.AnnouncementComments.First(i => i.AnnouncementCommentID == announcementCommentID);
            if (comment != null)
            {
                comment.IsFlagged = 1;
                dbContext.AnnouncementComments.Update(comment);
                dbContext.SaveChanges();
                return comment;
            } else
            {
                return null;
            }
        }

        public List<AnnouncementComment> GetAnnoucementComments(int houseAnnouncementID)
        {
            var data = dbContext.AnnouncementComments.Where(i => i.HouseAnnouncementID == houseAnnouncementID).ToList();
            return data;
        }
    }
}
