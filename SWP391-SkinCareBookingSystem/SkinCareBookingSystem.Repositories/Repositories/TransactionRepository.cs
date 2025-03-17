﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<Transaction> GetById(int id) =>
            await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);

        public async Task<bool> SaveChange() =>
            await _context.SaveChangesAsync() > 0;

        public void Update(Transaction transaction) =>
            _context.Transactions.Update(transaction);
    }
}
