using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.App.Data;
using Web.App.Models.Account;
using Web.App.Models.Lead;
using Web.App.Models.User;

namespace Web.App.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<ApplicationUser, UserModel>().ReverseMap();
            // Add other mappings here
        }

    }
}
