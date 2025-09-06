using AutoMapper;
using MyProfessionals.Data.Model;
using MyProfessionalss.Data.Model;
using MyProfessionalss.Data.Model.DTO;
using MyProfessionalss.Data.Model.Enums;
using System;
using System.Linq;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
      // CompanyAdmin <-> CompanyAdminDto
      CreateMap<CompanyAdmin, CompanyAdminDto>()
          .ForMember(dest => dest.CompanyName,
              opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : string.Empty))
          .ForMember(dest => dest.UserFullName,
              opt => opt.MapFrom(src =>
                  src.User != null
                      ? $"{src.User.FirstName} {src.User.LastName}".Trim()
                      : string.Empty));
      
      CreateMap<CompanyAdminDto, CompanyAdmin>()
          .ForMember(dest => dest.Company, opt => opt.Ignore())
          .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}
