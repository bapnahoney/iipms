using Abp.Zero.EntityFrameworkCore;
using HIPMS.Authorization.Roles;
using HIPMS.Authorization.Users;
using HIPMS.Models;
using HIPMS.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace HIPMS.EntityFrameworkCore
{
    public class HIPMSDbContext : AbpZeroDbContext<Tenant, Role, User, HIPMSDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public HIPMSDbContext(DbContextOptions<HIPMSDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<POMaster> POMaster { get; set; } = null!;
        public virtual DbSet<POItem> POItems { get; set; } = null!;
        public virtual DbSet<InspectionClearance> InspectionClearance { get; set; } = null!;
        public virtual DbSet<ICData> ICData { get; set; } = null!;
        public virtual DbSet<ICDocument> ICDocuments { get; set; } = null!;
        public virtual DbSet<DispatchClearance> DispatchClearance { get; set; } = null!;
        public virtual DbSet<DCData> DCData { get; set; } = null!;
        public virtual DbSet<DCDocument> DCDocuments { get; set; } = null!;
        public virtual DbSet<RFI> RFI { get; set; } = null!;
        public virtual DbSet<RFIData> RFIData { get; set; } = null!;
        public virtual DbSet<RFIDocument> RFIDocuments { get; set; } = null!;
        public virtual DbSet<SAPUserPOMap> SAPUserPOMap { get; set; } = null!;
        public virtual DbSet<NCRMaster> NCRMaster { get; set; } = null!;
        public virtual DbSet<NCRDocument> NCRDocuments { get; set; } = null!;
        public virtual DbSet<UserSession> UserSession { get; set; } = null!;

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<POItem>()
        //   .HasKey(tl => new { tl.POMasterId});
        //    modelBuilder.Entity<POItem>()
        //        .HasOne(t => t.POMaster);

        //    modelBuilder.Entity<POMaster>().HasMany(x => x.POItems).WithMany().Map(y =>
        //    {
        //        y.MapLeftKey((x => x.Id), "ParentID");
        //        y.MapRightKey((x => x.Id), "ChildID");

        //    });

        //    //modelBuilder.Entity<POMaster>(entity =>
        //    //{
        //    //    entity.HasQueryFilter(e => e.is == null);
        //    //});
        //}

    }

}
