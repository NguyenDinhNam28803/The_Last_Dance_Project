using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Models;

namespace The_Last_Dance_Project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // 1. Nhóm Khách hàng
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<Role> Roles { get; set; }

        // 2. Nhóm Audit (Lưu ý: Đã đổi tên thành Auditentity theo file của bạn)
        public DbSet<Auditentity> Auditentities { get; set; }
        public DbSet<AuditEntityKey> AuditEntityKeys { get; set; }
        public DbSet<AuditEntityKeyDefinition> AuditEntityKeyDefinitions { get; set; }

        // 3. Nhóm Danh mục hệ thống
        public DbSet<SystemCode> SystemCodes { get; set; }
        public DbSet<SystemCodeValue> SystemCodeValues { get; set; }

        // 4. Quản lý Token
        public DbSet<TokenBlacklist> TokenBlacklists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng Auditentity (Ánh xạ đúng vào bảng MTTRAN trong DB)
            modelBuilder.Entity<Auditentity>(entity => {
                entity.ToTable("MTTRAN");
                entity.HasKey(e => e.MtTranId);
                entity.Property(e => e.MtTranId).UseIdentityColumn();
            });

            // Cấu hình bảng AuditEntityKey (Chi tiết log)
            modelBuilder.Entity<AuditEntityKey>(entity => {
                entity.ToTable("AUDITENTITY_KEY");
                entity.HasKey(e => e.MtTranFldId);
                entity.Property(e => e.MtTranFldId).UseIdentityColumn();

                // Khóa ngoại liên kết tới Auditentity (MTTRAN)
                entity.HasOne<Auditentity>()
                    .WithMany()
                    .HasForeignKey(d => d.MtTranId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình định nghĩa Audit
            modelBuilder.Entity<AuditEntityKeyDefinition>(entity => {
                entity.ToTable("AUDITENTITY_KEY_DEFINITION");
                entity.HasKey(e => e.DefId);
                entity.Property(e => e.DefId).UseIdentityColumn();
            });

            // Cấu hình quan hệ SystemCode và SystemCodeValue
            modelBuilder.Entity<SystemCodeValue>(entity => {
                entity.ToTable("SYSTEMCODE_VALUE");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();

                entity.HasOne(d => d.SystemCode)
                    .WithMany(p => p.Values)
                    .HasForeignKey(d => d.SystemCodeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed SystemCode for Role and corresponding values so AuthService can lookup role display name
            modelBuilder.Entity<SystemCode>(entity => {
                entity.ToTable("SYSTEMCODE");
                entity.HasKey(e => e.SystemCodeId);
                entity.Property(e => e.SystemCodeId).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasData(
                    new SystemCode { SystemCodeId = "ROLE", Name = "Role definitions", Description = "Application roles" }
                );
            });

            modelBuilder.Entity<SystemCodeValue>(entity => {
                // Ensure table mapping/keys already configured above; add seed entries here
                entity.HasData(
                    new SystemCodeValue { Id = 1001, SystemCodeId = "ROLE", CodeValue = "ADMIN", DisplayValue = "Administrator", DisplayValueEn = "Administrator", OrderBy = 1, IsDefault = "N" },
                    new SystemCodeValue { Id = 1002, SystemCodeId = "ROLE", CodeValue = "USER", DisplayValue = "User", DisplayValueEn = "User", OrderBy = 2, IsDefault = "Y" },
                    new SystemCodeValue { Id = 1003, SystemCodeId = "ROLE", CodeValue = "MANAGER", DisplayValue = "Manager", DisplayValueEn = "Manager", OrderBy = 3, IsDefault = "N" }
                );
            });

            // Cấu hình Customer và Contact
            modelBuilder.Entity<CustomerContact>(entity => {
                entity.ToTable("CUSTOMER_CONTACT");
                entity.HasKey(e => e.ContactId);

                entity.HasOne<Customer>()
                    .WithMany()
                    .HasForeignKey(d => d.CustId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình Role
            modelBuilder.Entity<Role>(entity => {
                entity.ToTable("ROLE");
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleId).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(100);
                // Seed sample roles
                entity.HasData(
                    new Role { RoleId = "ADMIN", Name = "Administrator", Description = "System administrator with full permissions" },
                    new Role { RoleId = "USER", Name = "User", Description = "Default application user" },
                    new Role { RoleId = "MANAGER", Name = "Manager", Description = "User with managerial permissions" }
                );
            });

            // Cấu hình quan hệ Customer -> Role (RoleId)
            modelBuilder.Entity<Customer>(entity => {
                entity.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
