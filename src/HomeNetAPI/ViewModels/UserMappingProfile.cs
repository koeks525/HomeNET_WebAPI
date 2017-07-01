using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;
using AutoMapper;

namespace HomeNetAPI.ViewModels
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserMappingProfile>();
        }
    }
}
