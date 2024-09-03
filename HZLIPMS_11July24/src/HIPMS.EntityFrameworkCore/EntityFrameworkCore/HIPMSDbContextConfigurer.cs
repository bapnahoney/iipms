using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace HIPMS.EntityFrameworkCore
{
    public static class HIPMSDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<HIPMSDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<HIPMSDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
