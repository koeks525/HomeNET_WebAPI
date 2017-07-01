using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private HomeNetContext homeContext;

        public OrganizationRepository(HomeNetContext homeContext)
        {
            this.homeContext = homeContext;
        }

        public Organization AddOrganization(Organization newOrganization)
        {
            var result = homeContext.Organizations.Add(newOrganization);
            homeContext.SaveChanges();
            if (result != null)
            {
                return result.Entity;
            } else
            {
                return null;
            }
        }

        public Organization DeleteOrganization(int organizationID)
        {
            var foundOrg = homeContext.Organizations.FirstOrDefault(i => i.OrganizationID == organizationID);
            if (foundOrg != null)
            {
                foundOrg.IsDeleted = 1;
                homeContext.SaveChanges();
                return foundOrg;
            } else
            {
                return null;
            }
        }

        public List<Organization> GetOrganizations()
        {
            return homeContext.Organizations.ToList();
        }

        public Organization UpdateOrganization(Organization updateOrganization)
        {
            var foundOrganization = homeContext.Organizations.FirstOrDefault(i => i.OrganizationID == updateOrganization.OrganizationID);
            if (foundOrganization != null)
            {
                foundOrganization = updateOrganization; //Lazy way
                homeContext.SaveChanges();
                return foundOrganization;
            } else
            {
                return null;
            }
        }

        public Organization GetOrganization(int organizationID)
        {
            var foundOrganization = homeContext.Organizations.Where(i => i.IsDeleted == 0).FirstOrDefault(p => p.OrganizationID == organizationID);
            return foundOrganization;
        }

        public List<Organization> GetUserOrganizations(int userId)
        {
            var foundUser = homeContext.Users.FirstOrDefault(u => u.Id == userId);
            if (foundUser != null)
            {
                var organizations = homeContext.Organizations.Where(i => i.UserID == userId).ToList();
                return organizations;
            } else
            {
                return null;
            }
        }
    }
}
