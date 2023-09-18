using AutoMapper;
using TimeLogger.Business.Model;
using TimeLogger.Component.Models.Security;
using TimeLogger.Component.Security.Entities;
using TimeLogger.Data.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Configuration.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<Microsoft.AspNetCore.Authentication.AuthenticationScheme, AuthenticationSchemeModel>().ReverseMap();

            CreateMap<UserIdentity, IdentityUserModel>().ReverseMap();
            CreateMap<UserIdentity, IdentityUserResponseModel>().ForMember(dest => dest.HasPassword, opt => opt.MapFrom(srs => !string.IsNullOrWhiteSpace(srs.PasswordHash)))
                                                                .ForMember(dest => dest.Roles, opt => opt.MapFrom(srs => srs.UserRoles.Select(x => x.Role.Name)))
                                                                .ReverseMap();
            CreateMap<UserIdentity, UpdateUserModel>().ReverseMap();
            CreateMap<UserIdentity, UserAuthenticationInfoModel>().ForMember(dest => dest.HasPassword, opt => opt.MapFrom(srs => !string.IsNullOrWhiteSpace(srs.PasswordHash)))
                                                                  .ReverseMap();

            CreateMap<ApplicationUser, UserModel>().ReverseMap();
            CreateMap<ApplicationUser, UserResponseModel>().ReverseMap();

            CreateMap<UserModel, UpdateUserModel>().ReverseMap();
            CreateMap<UserResponseModel, UpdateUserModel>().ReverseMap();

            CreateMap<TwoFactorType, TwoFactorTypeModel>().ReverseMap();

            CreateMap<UserLoginInfo, UserLoginInfoModel>().ReverseMap();
            CreateMap<UserIdentityLogin, UserLoginInfoModel>().ReverseMap();

            CreateMap<Company, CompanyModel>().ReverseMap();
            CreateMap<Company, CompanyResponseModel>().ReverseMap();

            CreateMap<Addresses, AddressModel>().ReverseMap();
            CreateMap<Addresses, AddressResponseModel>().ReverseMap();

            CreateMap<Country, CountryModel>().ReverseMap();
            CreateMap<City, CityModel>().ReverseMap();
        }
    }
}
