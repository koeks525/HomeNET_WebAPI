using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private HomeNetContext homeContext;

        public UserRepository(HomeNetContext homeContext)
        {
            this.homeContext = homeContext;
        }
        public User CreateUser(User newUser)
        {
            var user = homeContext.Users.Add(newUser);
            homeContext.SaveChanges();
            newUser.Id = user.Entity.Id;
            return newUser;
        }

        public User DeleteUser(int userId)
        {
            var foundUser = homeContext.Users.FirstOrDefault(u => u.Id == userId);
            foundUser.IsDeleted = 1; //Set to deleted
            homeContext.SaveChanges();
            return foundUser;

        }

        public User GetUser(string emailAddress)
        {
            var foundUser = homeContext.Users.FirstOrDefault(u => u.Email == emailAddress);
            homeContext.SaveChanges();
            return foundUser;
        }

        public User GetUser(string emailAddress, string dateOfBirth)
        {
            var foundUser = homeContext.Users.FirstOrDefault(u => u.Email == emailAddress && u.DateOfBirth == dateOfBirth);
            homeContext.SaveChanges();
            return foundUser;
        }

        public User UpdateUserAccount(User updatedUser)
        {
            var updateUser = homeContext.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            homeContext.SaveChanges();
            updateUser = updatedUser;
            return updateUser;

        }

        public User GetUserByUsername(string username)
        {
            var user = homeContext.Users.FirstOrDefault(u => u.UserName.ToUpper().Trim() == username.ToUpper().Trim());
            homeContext.SaveChanges();
            return user;
        }

        public User RemoveUser(int userId)
        {
            var user = homeContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                homeContext.Users.Remove(user);
                homeContext.SaveChanges();
                return user;
            } else
            {
                return null;
            }
        }

        public List<HousePost> GetHousePosts(int userId)
        {
            List<HousePost> UserPosts = new List<HousePost>();
            List<HouseMember> MembershipList = homeContext.HouseMembers.Where(u => u.UserID == userId).ToList();
            if (MembershipList != null)
            {
                foreach (HouseMember Member in MembershipList)
                {
                    var result = homeContext.HousePosts.Where(i => i.HouseMemberID == Member.HouseMemberID).ToList();
                    if (result != null)
                    {
                        foreach (HousePost Post in result)
                        {
                            UserPosts.Add(Post);
                        }
                    }
                }
                return UserPosts;
            } else
            {
                return null;
            }
        }
    }
}
