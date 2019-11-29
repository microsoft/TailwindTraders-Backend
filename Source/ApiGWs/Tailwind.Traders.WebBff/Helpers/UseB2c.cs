using Microsoft.Extensions.Configuration;

namespace Tailwind.Traders.WebBff.Helpers
{
    public static class UseBc2
    {
        public static  bool GetUseB2CBoolean(IConfiguration _configuration)
        {
            string useB2C = _configuration["UseB2C"];

            if (useB2C == null)
            {
                return false;
            }

            if (bool.TryParse(useB2C, out bool parsedUseB2C))
            {
                return parsedUseB2C;
            }

            return false;
        }
    }
}
