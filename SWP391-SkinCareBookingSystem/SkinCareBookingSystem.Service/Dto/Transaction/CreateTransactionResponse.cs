using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.Transaction
{
    public class CreateTransactionResponse
    {
        public CreatePaymentResult createPaymentResult {  get; set; }
        public int transactionId { get; set; }
    }
}
