using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        public void Create(Transaction transaction);
        public Task<bool> SaveChange();
        public Task<Transaction> GetById(int id);
        public Task<List<Transaction>> GetByUserId(int id);
        public void Update(Transaction transaction);    
        public Task<Transaction> GetTransactionByABookingIdAndUserId(int userId, int bookingId);    
        public Task<List<Transaction>> GetAllTransactions();    
    }
}
