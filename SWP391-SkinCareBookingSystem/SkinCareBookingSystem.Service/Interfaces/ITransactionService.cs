using Net.payOS.Types;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface ITransactionService
    {
        public Task<CreatePaymentResult> CreateTransaction(Booking booking);
        public Task<CreatePaymentResult> CreateTransaction(CheckoutCartRequest request);
    }
}
