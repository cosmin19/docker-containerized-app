using AutoMapper;
using Enviroself.Areas.Admin.Features.User.Dto;
using Enviroself.Areas.User.Features.Account.Entities;
using Enviroself.Areas.User.Features.User.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Infrastructure.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserAdminDto>()
                .ForMember(d => d.CreatedOnUtc, opt => opt.MapFrom(s => s.CreatedOnUtc.ToString()))
                .ForMember(d => d.Role, opt => opt.Ignore())
                .ForMember(d => d.RoleList, opt => opt.Ignore())
                .ForMember(d => d.GenderList, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserSmallDto>()
                .ForMember(c => c.GenderList, opt => opt.Ignore());

        }
    }
}
