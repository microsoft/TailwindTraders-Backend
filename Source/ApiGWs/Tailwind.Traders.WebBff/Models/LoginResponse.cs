using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.WebBff.Models
{
    public class LoginResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }
}
