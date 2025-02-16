using Net.payOS.Types;
using SkinCareBookingSystem.BusinessObject.Entity;
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
    }
}
