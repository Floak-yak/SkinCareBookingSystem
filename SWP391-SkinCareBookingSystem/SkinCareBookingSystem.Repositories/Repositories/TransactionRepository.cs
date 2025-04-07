using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Data;
using SkinCareBookingSystem.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context) => _context = context;
        public void Create(Transaction transaction) =>
            _context.Add(transaction);

        public async Task<List<Transaction>> GetAllTransactions() =>
            await _context.Transactions.Include(t => t.User).Include(t => t.Products).OrderByDescending(t => t.CreatedDate).ToListAsync();

        public async Task<Transaction> GetById(int id) =>
            await _context.Transactions.Include(t => t.User).Include(t => t.Products).FirstOrDefaultAsync(t => t.Id == id);

        public async Task<List<Transaction>> GetByUserId(int id) =>
            await _context.Transactions.Include(t => t.User).Include(t => t.Products).Where(t => t.UserId == id).ToListAsync();

        public async Task<Transaction> GetTransactionByABookingIdAndUserId(int userId, int bookingId) =>
            await _context.Transactions
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.User.Bookings
                    .FirstOrDefault(b => b.Id == bookingId) != null && t.UserId == userId);

        public async Task<Transaction> GetTransactionByBookingId(int bookingId) =>
            await _context.Transactions.FirstOrDefaultAsync(t => t.BookingId == bookingId);

        public async Task<List<Transaction>> GetTransactionsByBookingId(List<int> bookingIds) =>
            await _context.Transactions.Where(t => t.BookingId.HasValue && bookingIds.Contains(t.BookingId.Value)).ToListAsync();

        public async Task<bool> SaveChange() =>
            await _context.SaveChangesAsync() > 0;

        public void Update(Transaction transaction)
        {
            if (transaction != null)
            {
                if (_context.Attach(transaction).State == EntityState.Modified)
                    _context.Transactions.Update(transaction);
            }            
        }
            
    }
}
