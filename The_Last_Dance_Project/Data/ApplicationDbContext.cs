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

        // 2. Nhóm Audit (Lưu ý: Đã đổi tên thành Auditentity theo file của bạn)
        public DbSet<Auditentity> Auditentities { get; set; }
        public DbSet<AuditEntityKey> AuditEntityKeys { get; set; }
        public DbSet<AuditEntityKeyDefinition> AuditEntityKeyDefinitions { get; set; }

        // 3. Nhóm Danh mục hệ thống
        public DbSet<SystemCode> SystemCodes { get; set; }
        public DbSet<SystemCodeValue> SystemCodeValues { get; set; }

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

            // Cấu hình Customer và Contact
            modelBuilder.Entity<CustomerContact>(entity => {
                entity.ToTable("CUSTOMER_CONTACT");
                entity.HasKey(e => e.ContactId);

                entity.HasOne<Customer>()
                    .WithMany()
                    .HasForeignKey(d => d.CustId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}