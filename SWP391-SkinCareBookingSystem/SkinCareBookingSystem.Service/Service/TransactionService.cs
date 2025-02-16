﻿using Net.payOS;
using Net.payOS.Types;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly PayOS _payOS;

        public TransactionService(PayOS payOS, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
            _payOS = payOS;
        }
        public async Task<CreatePaymentResult> CreateTransaction(Booking booking)
        {
            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            ItemData item = new ItemData(booking.BookingServiceSchedules.FirstOrDefault().Service.ServiceName, 1, decimal.ToInt32(booking.TotalPrice));
            var cancelUrl = "http://localhost:5173/payment-cancelled";
            var returnUrl = "http://localhost:5173/thank-you";
            List<ItemData> items = new List<ItemData>();
            items.Add(item);
            PaymentData paymentData = new PaymentData(orderCode, decimal.ToInt32(booking.TotalPrice), "Thanh toan doan hang", items, cancelUrl, returnUrl);

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
            return createPayment;
        }
    }
}
