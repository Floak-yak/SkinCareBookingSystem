using AutoMapper;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.BusinessObject.Helper;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.BookingDto;
using SkinCareBookingSystem.Service.Dto.ImageDto;
using SkinCareBookingSystem.Service.Dto.Product;
using SkinCareBookingSystem.Service.Dto.Schedule;
using SkinCareBookingSystem.Service.Dto.ScheduleLog;
using SkinCareBookingSystem.Service.Dto.Survey;
using SkinCareBookingSystem.Service.Dto.Transaction;
using SkinCareBookingSystem.Service.Dto.User;

namespace SkinCareBookingSystem.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, ViewUser>().ForMember(d => d.PaymentNumber, 
                            opt => opt.MapFrom(src => src.PaymentNumber != null
                           ? EncryptionHelper.Decrypt(src.PaymentNumber)
                           : null)).ReverseMap();
            CreateMap<GetBookingsResponse, Booking>().ReverseMap();
            CreateMap<GetProductResponse, Product>().ReverseMap();
            CreateMap<User, UserResponse>().ReverseMap();
            CreateMap<User, CreateAccountResponse>().ReverseMap();
            CreateMap<Product, CreateProductRequest>().ReverseMap();
            CreateMap<User, SkinTherapistResponse>().ReverseMap();
            CreateMap<Image, StoreImageResponse>().ReverseMap();
            CreateMap<Transaction, GetTransactionResponse>().ReverseMap();
            CreateMap<Schedule, ScheduleResponse>().ReverseMap();
            CreateMap<ScheduleLog, ScheduleLogResponse>().ReverseMap();
            CreateMap<ScheduleLog, ScheduleLogDto>().ReverseMap();
            CreateMap<BookingServiceSchedule, BookingServiceScheduleResponse>().ReverseMap();
            CreateMap<SurveyOption, OptionUpdateDto>().ReverseMap();
            CreateMap<SkinTypePointsDto, OptionSkinTypePoints>().ReverseMap();
        }
    }
}
