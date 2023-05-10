using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Stargazer.Abp.Template.EntityFrameworkCore
{
    public static class EntityFrameworkCoreDbContextModelCreatingExtensions
    {
        public static void Configure(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));
        }
    }
}