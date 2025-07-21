using AutoMapper;
using RentReady.API.Dtos;
using RentReady.API.Dtos.Property;
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
        }
    }
}