using AutoMapper;
using NextOnServices.Core.Entities;
using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Services.Configurations;

public class MapperInitializer : Profile
{
    public MapperInitializer()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<Project, ProjectDTO>().ReverseMap();
        CreateMap<Client, ClientDTO>().ReverseMap();
        CreateMap<CountryMaster, CountryMasterDTO>().ReverseMap();
        CreateMap<StatusMaster, StatusMasterDTO>().ReverseMap();
        CreateMap<ProjectMapping, ProjectMappingDTO>().ReverseMap();
        CreateMap<ProjectsUrl, ProjectsUrlDTO>().ReverseMap();
        CreateMap<Supplier, SupplierDTO>().ReverseMap();
        CreateMap<SupplierPanelSize, SupplierPanelSizeDTO>().ReverseMap();
        CreateMap<SupplierLogin, SupplierLoginDTO>().ReverseMap();
    }
}
