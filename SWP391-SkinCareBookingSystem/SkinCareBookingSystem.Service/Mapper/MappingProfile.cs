using AutoMapper;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.ImageDto;
using SkinCareBookingSystem.Service.Dto.Product;
using SkinCareBookingSystem.Service.Dto.Transaction;
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
            CreateMap<User, CreateAccountResponse>().ReverseMap();
            CreateMap<Product, CreateProductRequest>().ReverseMap();
            CreateMap<User, SkinTherapistResponse>().ReverseMap();
            CreateMap<Image, StoreImageResponse>().ReverseMap();
            CreateMap<Transaction, GetTransactionResponse>().ReverseMap();
        }
    }
}
