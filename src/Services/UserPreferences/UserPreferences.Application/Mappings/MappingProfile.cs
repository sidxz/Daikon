using AutoMapper;
using UserPreferences.Application.Features.Commands.SetTableDefaults;
using UserPreferences.Application.Features.Commands.SetTableGlobal;
using UserPreferences.Application.Features.Commands.SetTableUserCustom;
using UserPreferences.Domain.Table;

namespace UserPreferences.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Table Defaults
            CreateMap<TableDefaults, SetTableDefaultsCommand>().ReverseMap();

            // Table Global Config
            CreateMap<TableGlobalConfig, SetTableGlobalConfigCommand>().ReverseMap();

            // Table User Customization
            CreateMap<TableUserCustomization, SetTableUserCustomizationCommand>().ReverseMap();

            // Self-clone maps (optional but often useful for updates)
            CreateMap<TableDefaults, TableDefaults>();
            CreateMap<TableGlobalConfig, TableGlobalConfig>();
            CreateMap<TableUserCustomization, TableUserCustomization>();
        }
    }
}
