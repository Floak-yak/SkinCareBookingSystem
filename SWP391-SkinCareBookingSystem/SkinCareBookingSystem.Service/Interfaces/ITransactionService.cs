using Net.payOS.Types;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto.Product;
using SkinCareBookingSystem.Service.Dto.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction = SkinCareBookingSystem.BusinessObject.Entity.Transaction;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface ITransactionService
    {
        public Task<CreateTransactionResponse> CreateTransaction(Booking booking);
        public Task<CreateTransactionResponse> CreateTransaction(CheckoutCartRequest request);
        public Task<bool> UpdateTransaction(int transactionId, int status);
        public Task<List<GetTransactionResponse>> GetTransactionByUserId(int userId); 
        public Task<List<GetTransactionResponse>> GetAllTransactions();
        public Task<Transaction> GetTransactionByBookingId(int bookingId);
        public Task<Transaction> GetTransactionById(int transactionId);
    }
}
