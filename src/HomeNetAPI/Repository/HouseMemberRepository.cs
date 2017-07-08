using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class HouseMemberRepository : IHouseMemberRepository
    {

        private HomeNetContext dbContext;

        public HouseMemberRepository(HomeNetContext dbContext)
        {
            this.dbContext = dbContext;
        }

        
        public HouseMember AddHouseMember(HouseMember newHouseMeber)
        {
            if (newHouseMeber.House == null)
            {
                var result = dbContext.Houses.FirstOrDefault(i => i.HouseID == newHouseMeber.HouseID);
                newHouseMeber.House = result;
            }

            var addResult = dbContext.HouseMembers.Add(newHouseMeber);
            if (addResult != null)
            {
                dbContext.SaveChanges();
                return addResult.Entity;
            } else
            {
                dbContext.SaveChanges();
                return null;
            }
        }

        public HouseMember ApproveHouseMember(int houseMemberId)
        {
            var houseMembership = dbContext.HouseMembers.FirstOrDefault(i => i.HouseMemberID == houseMemberId);
            if (houseMembership != null)
            {
                houseMembership.ApprovalStatus = 1;
                houseMembership.DateApproved = DateTime.Now.ToString();
                dbContext.SaveChanges();
                return houseMembership;
            } else
            {
                return null;
            }
        }

        public HouseMember BanHouseMember(int houseMember)
        {
            var result = dbContext.HouseMembers.FirstOrDefault(i => i.HouseMemberID == houseMember);
            if (result != null)
            {
                result.ApprovalStatus = 2;
                dbContext.SaveChanges();
                return result;
            } else
            {
                return null;
            }
        }

        public List<HouseMember> GetActiveHouseMembers(int houseID)
        {
            var result = dbContext.HouseMembers.Where(i => i.HouseID == houseID && i.ApprovalStatus == 0).ToList();
            return result;
        }

        public List<HouseMember> GetBannedHouseMembers(int houseID)
        {
            var result = dbContext.HouseMembers.Where(i => i.HouseID == houseID && i.ApprovalStatus == 2).ToList();
            return result;
        }

        public List<HouseMember> GetHouseMember(int userId)
        {
            return dbContext.HouseMembers.Where(i => i.UserID == userId).ToList();
        }

        public List<HouseMember> GetHouseMembers()
        {
            return dbContext.HouseMembers.ToList();
        }

        public HouseMember GetHouseMembership(int houseMemberID)
        {
            return dbContext.HouseMembers.First(i => i.HouseMemberID == houseMemberID);
        }

        public List<HouseMember> GetPendingHouseMembers(int houseID)
        {
            var result = dbContext.HouseMembers.Where(i => i.HouseID == houseID && i.ApprovalStatus == 1).ToList();
            return result;
        }

        public HouseMember RemoveHouseMember(int houseMember)
        {
            var houseMemberShip = dbContext.HouseMembers.First(i => i.HouseMemberID == houseMember);
            if (houseMemberShip != null)
            {
                houseMemberShip.IsDeleted = 1;
                houseMemberShip.DateLeft = DateTime.Now.ToString();
                dbContext.SaveChanges();
                return houseMemberShip;
            } else
            {
                return null;
            }
        }

        public HouseMember UpdateMembership(HouseMember selectedMembership)
        {
            var membership = dbContext.HouseMembers.First(i => i.HouseMemberID == selectedMembership.HouseMemberID);
            membership = selectedMembership;
            dbContext.SaveChanges();
            return membership;
        }

        public List<HouseMember> GetHouseMemberships(int houseID)
        {
            var memberships = dbContext.HouseMembers.Where(i => i.HouseID == houseID && i.IsDeleted == 0).ToList();
            return memberships;
        }

        public HouseMember GetMembership(int houseID, int userID)
        {
            var membership = dbContext.HouseMembers.First(i => i.UserID == userID && i.HouseID == houseID);
            return membership;
        }
    }
}
