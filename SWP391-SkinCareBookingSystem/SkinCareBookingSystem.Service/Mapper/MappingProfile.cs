using AutoMapper;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;

namespace SkinCareBookingSystem.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, ViewUser>().ReverseMap();
            CreateMap<GetBookingsResponse, Booking>().ReverseMap();
            CreateMap<GetProductResponse, Product>().ReverseMap();
        }
    }
}
