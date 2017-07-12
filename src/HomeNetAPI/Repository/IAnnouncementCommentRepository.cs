using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface IAnnouncementCommentRepository
    {
        AnnouncementComment AddAnnouncementComment(AnnouncementComment newOne);
        AnnouncementComment FlagAnnouncementComment(int announcementCommentID);
        List<AnnouncementComment> GetAnnoucementComments(int houseAnnouncementID);
    }
}
