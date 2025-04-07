using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.BookingDto
{
    public class UploadImageForTransactionRequest
    {
        public int BookingId {  get; set; } 
        public IFormFile Image {  get; set; }
    }
}
