using System.Collections.Generic;
using HomeNetAPI.Models;
namespace HomeNetAPI.Repository
{
    public interface IUserRepository
    {
        User CreateUser(User newUser);
        User DeleteUser(int userId);
        User GetUser(string emailAddress);
        User UpdateUserAccount(User updateUser);
        User GetUserByUsername(string username);
        User RemoveUser(int userId);
        User GetUser(string emailAddress, string dateOfBirth);
        List<HousePost> GetHousePosts(int userId);
    }
}
