using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private HomeNetContext homeContext;

        public AnnouncementRepository(HomeNetContext homeContext)
        {
            this.homeContext = homeContext;
        }

        public List<HouseAnnouncement> GetHouseAnnouncements(int houseID)
        {
            return homeContext.HouseAnnouncements.Where(h => h.HouseID == houseID && h.IsDeleted == 0).ToList();
        }

        public List<HouseAnnouncement> GetUserAnnoucements(int houseMemberID)
        {
            var announcements = homeContext.HouseAnnouncements.Where(i => i.HouseMemberID == houseMemberID && i.IsDeleted == 0).ToList();
            return announcements;
        }

        public HouseAnnouncement AddHouseAnnouncement(HouseAnnouncement newAnnouncement)
        {
            var result = homeContext.HouseAnnouncements.Add(newAnnouncement);
            homeContext.SaveChanges();
            return result.Entity;
        }

        public HouseAnnouncement GetHouseAnnouncement(int houseAnnouncementID)
        {
            var result = homeContext.HouseAnnouncements.First(i => i.HouseAnnouncementID == houseAnnouncementID);
            return result;
        }

        public List<HouseAnnouncement> GetAnnouncementsByMembership(int houseMemberID)
        {
            var results = homeContext.HouseAnnouncements.Where(i => i.HouseMemberID == houseMemberID).ToList();
            return results;
        }
    }
}
