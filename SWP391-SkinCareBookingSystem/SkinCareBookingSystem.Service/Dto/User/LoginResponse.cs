using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.User
{
	public class LoginResponse
	{
		public string Token { get; set; }
		public int UserId { get; set; }
	}
}
