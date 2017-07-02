using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface IHouseMemberRepository
    {
        List<HouseMember> GetHouseMembers();
        List<HouseMember> GetHouseMember(int userId);
        HouseMember AddHouseMember(HouseMember newHouseMeber);
        HouseMember ApproveHouseMember(int houseMemberId);
        HouseMember BanHouseMember(int houseMember);
        HouseMember RemoveHouseMember(int houseMember);
        List<HouseMember> GetActiveHouseMembers(int houseID); //0
        List<HouseMember> GetPendingHouseMembers(int houseID); //1
        List<HouseMember> GetBannedHouseMembers(int houseID); //2
        HouseMember GetHouseMembership(int houseMemberID);
        HouseMember UpdateMembership(HouseMember selectedMembership);
        List<HouseMember> GetHouseMemberships(int houseID);
        
    }
}
