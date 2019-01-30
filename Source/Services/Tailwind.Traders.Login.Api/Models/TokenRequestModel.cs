using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.Login.Api.Models
{
    public class TokenRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonProperty(PropertyName = "grant_type")]
        public string GrantType { get; set; }
    }
}
