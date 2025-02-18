using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Transaction
    {
        public int Id { get; set; }
        public Decimal TotalMoney { get; set; }
        public DateTime CreatedDate { get; set; }
        public TranctionType TranctionType { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
    public enum TranctionType
    {
        AccountAssignment = 1,
        UnDefined = 0,
    }
}
