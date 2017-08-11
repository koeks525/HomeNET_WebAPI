using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface ICommentRepository
    {
        List<HousePostComment> GetComments(int housePostID);
        HousePostComment AddComment(HousePostComment newComment);
        HousePostComment DeleteComment(int housePostCommentID);
        HousePostComment GetComment(int housePostCommentID);

    }
}
