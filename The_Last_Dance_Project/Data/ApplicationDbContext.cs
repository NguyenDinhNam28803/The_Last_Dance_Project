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

        // 2. Nhóm Audit
        public DbSet<AuditEntity> AuditEntities { get; set; }
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

            // Seed Role data
            modelBuilder.Entity<Role>(entity => {
                entity.ToTable("ROLE");
                entity.HasKey(e => e.RoleId);
                entity.HasData(
                    new Role { RoleId = "MAKER", Name = "Maker", Description = "Create and modify requests" },
                    new Role { RoleId = "CHECKER", Name = "Checker", Description = "Approve or reject requests" },
                    new Role { RoleId = "ADMIN", Name = "Admin", Description = "Approve or reject requests and Manage System" }
                );
            });

            // Cấu hình bảng AuditEntity (Ánh xạ đúng vào bảng MTTRAN trong DB)
            modelBuilder.Entity<AuditEntity>(entity => {
                entity.ToTable("MTTRAN");
                entity.HasKey(e => e.MtTranId);
                entity.Property(e => e.MtTranId).UseIdentityColumn();
            });

            // Cấu hình bảng AuditEntityKey (Chi tiết log)
            modelBuilder.Entity<AuditEntityKey>(entity => {
                entity.ToTable("AUDITENTITY_KEY");
                entity.HasKey(e => e.MtTranFldId);
                entity.Property(e => e.MtTranFldId).UseIdentityColumn();

                // Khóa ngoại liên kết tới AuditEntity (MTTRAN)
                entity.HasOne<AuditEntity>()
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
                    new SystemCode { SystemCodeId = "ROLE", Name = "Role definitions", Description = "Application roles" },
                    new SystemCode { SystemCodeId = "STATUS", Name = "Transaction Status", Description = "Statuses for MTTRAN" }
                );
            });

            modelBuilder.Entity<SystemCodeValue>(entity => {
                // Ensure table mapping/keys already configured above; add seed entries here
                entity.HasData(
                    new SystemCodeValue { Id = 1001, SystemCodeId = "ROLE", CodeValue = "ADMIN", DisplayValue = "Administrator", DisplayValueEn = "Administrator", OrderBy = 1, IsDefault = "N" },
                    new SystemCodeValue { Id = 1002, SystemCodeId = "ROLE", CodeValue = "USER", DisplayValue = "User", DisplayValueEn = "User", OrderBy = 2, IsDefault = "Y" },
                    new SystemCodeValue { Id = 1003, SystemCodeId = "ROLE", CodeValue = "MANAGER", DisplayValue = "Manager", DisplayValueEn = "Manager", OrderBy = 3, IsDefault = "N" },
                    new SystemCodeValue { Id = 1004, SystemCodeId = "ROLE", CodeValue = "MAKER", DisplayValue = "Maker", DisplayValueEn = "Maker", OrderBy = 4, IsDefault = "N" },
                    new SystemCodeValue { Id = 1005, SystemCodeId = "ROLE", CodeValue = "CHECKER", DisplayValue = "Checker", DisplayValueEn = "Checker", OrderBy = 5, IsDefault = "N" }
                );
            });

            // Cấu hình Customer
            modelBuilder.Entity<Customer>(entity => {
                entity.ToTable("Customers");
                entity.HasKey(e => e.CustId);

                // Mật khẩu "password123" băm bằng BCrypt
                string hashedPw = "$2a$11$qR7i5J2k5W.O7pL7X/f8/.hHqXmZ.xH1v5B8j0C9iE8A1D1G1I1K1"; 
                var staticDate = new DateTime(2026, 3, 27, 0, 0, 0, DateTimeKind.Utc);

                entity.HasData(
                    new Customer { 
                        CustId = "ACC_MAKER_001", UserName = "maker_system", Name = "Maker System Account", 
                        RoleId = "MAKER", Status = "Active", PasswordHash = hashedPw, RecordStatus = "1",
                        Email = "maker_sys@test.com", CreatedDate = staticDate 
                    },
                    new Customer { 
                        CustId = "ACC_CHECKER_001", UserName = "checker_system", Name = "Checker System Account", 
                        RoleId = "CHECKER", Status = "Active", PasswordHash = hashedPw, RecordStatus = "1",
                        Email = "checker_sys@test.com", CreatedDate = staticDate 
                    }
                );

                entity.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
