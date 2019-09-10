using CsvHelper.Configuration;
using Tailwind.Traders.Profile.Api.Csv;

namespace Tailwind.Traders.Profile.Api.Helpers
{
    public sealed class ProfilesMap : ClassMap<ProfileData>
    {
        public ProfilesMap()
        {
            AutoMap();
        }
    }
}
