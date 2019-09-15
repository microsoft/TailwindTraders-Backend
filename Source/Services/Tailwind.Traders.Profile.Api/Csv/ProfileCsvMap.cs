using CsvHelper.Configuration;
using Tailwind.Traders.Profile.Api.Csv;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Helpers
{
    public sealed class ProfilesMap : ClassMap<ProfileData>
    {
        public ProfilesMap()
        {
            AutoMap();
            Map(m => m.Id).Ignore();
        }
    }
}
