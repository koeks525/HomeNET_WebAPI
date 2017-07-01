using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface IOrganizationRepository
    {
        Organization AddOrganization(Organization newOrganization);
        Organization DeleteOrganization(int organizationID);
        List<Organization> GetOrganizations();
        Organization UpdateOrganization(Organization updateOrganization);
        Organization GetOrganization(int organizationId);
        List<Organization> GetUserOrganizations(int userId);
    }
}
