using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;

namespace SkinCareBookingSystem.BusinessObject.Mapper
{
    public static class ServicesDetailMapper
    {
        public static ServicesDetailResponseDTO ToDTO(this ServicesDetail entity)
        {
            if (entity == null) return null;

            return new ServicesDetailResponseDTO
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Duration = entity.Duration,
                ImageId = entity.Image?.Id,
                ServiceId = entity.ServiceId,
                ServiceName = entity.SkincareService?.ServiceName
            };
        }

        public static ServicesDetail ToEntity(this CreateServicesDetailDTO dto)
        {
            if (dto == null) return null;

            var entity = new ServicesDetail
            {
                Title = dto.Title,
                Description = dto.Description,
                Duration = dto.Duration,
                ServiceId = dto.ServiceId
            };

            if (dto.ImageId.HasValue)
            {
                entity.Image = new Image { Id = dto.ImageId.Value };
            }

            return entity;
        }

        public static void UpdateEntity(this UpdateServicesDetailDTO dto, ServicesDetail entity)
        {
            if (dto == null || entity == null) return;

            if (!string.IsNullOrWhiteSpace(dto.Title))
                entity.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                entity.Description = dto.Description;

            if (dto.Duration > 0)
                entity.Duration = dto.Duration;

            if (dto.ServiceId > 0)
                entity.ServiceId = dto.ServiceId;

            if (dto.ImageId.HasValue)
            {
                entity.Image = new Image { Id = dto.ImageId.Value };
            }
        }
    }
} 