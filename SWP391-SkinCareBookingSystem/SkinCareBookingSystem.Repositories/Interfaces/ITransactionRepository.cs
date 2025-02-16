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
    }
}
