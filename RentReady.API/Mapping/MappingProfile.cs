using AutoMapper;
using RentReady.API.Dtos;

using RentReady.API.Models;

namespace RentReady.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity → DTO
            CreateMap<Property, PropertyDto>();

            // DTO → Entity
            CreateMap<PropertyForEditDto, Property>();

            // Lease mappings
            CreateMap<LeaseForEditDto, Lease>();
            CreateMap<Lease, LeaseDto>()
                .ForMember(dest => dest.Property, opt => opt.MapFrom(src => src.Property))
                .ForMember(dest => dest.Tenant, opt => opt.MapFrom(src => src.Tenant));

            // Tenant mappings
            CreateMap<TenantForEditDto, Tenant>();
            CreateMap<Tenant, TenantDto>()
                .ForMember(dest => dest.Leases, opt => opt.MapFrom(src => src.Leases));
        }
    }
}
