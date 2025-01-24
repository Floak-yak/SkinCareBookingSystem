using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class TestInformation
    {
        public int Id { get; set; }
        public TypeOfTest TestType { get; set; }
        public string SkinStatus { get; set; }
        public DateTime TestDate { get; set; }

        #region Relationship
        public int UserId { get; set; }
        public User User { get; set; }
        #endregion
    }

    public enum TypeOfTest
    {
        IntradermalTest = 1,
        SkinTest = 2,
        SkinPrickTest = 3
    }
}
