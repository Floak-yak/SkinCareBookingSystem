using AutoMapper;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.User;

namespace SkinCareBookingSystem.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, ViewUser>().ReverseMap();
            CreateMap<GetBookingsResponse, Booking>().ReverseMap();
            CreateMap<GetProductResponse, Product>().ReverseMap();
            CreateMap<User, UserResponse>().ReverseMap();
        }
    }
}
