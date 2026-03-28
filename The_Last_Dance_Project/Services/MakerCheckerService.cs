using System;
using System.Threading.Tasks;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Models;
using Microsoft.EntityFrameworkCore;
using The_Last_Dance_Project.Interfaces;

namespace The_Last_Dance_Project.Services
{
    public class MakerCheckerService : IMakerCheckerService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICustomerContactService _contactService;

        public MakerCheckerService(ApplicationDbContext db, ICustomerContactService contactService)
        {
            _db = db;
            _contactService = contactService;
        }

        public async Task<IEnumerable<AuditEntity>> GetPendingRequestsAsync()
        {
            return await _db.AuditEntities
                .Where(a => a.MtlStatus == "N")
                .OrderByDescending(a => a.ActionDate)
                .ToListAsync();
        }

        // MAKER: Tạo yêu cầu mới
        public async Task<bool> SubmitRequestAsync(string entityName, string entityId, string action, string makerId, string details)
        {
            var auditEntity = new AuditEntity
            {
                ObjChange = entityName,
                KeyField = entityId,
                MtlType = action, // "CREATE", "UPDATE", "DELETE"
                MtlStatus = "N", // N: Chờ duyệt (Pending)
                Maker = makerId,
                ActionDate = DateTime.UtcNow,
                BusDate = DateTime.UtcNow,
                ModCode = "SYS",
                Description = details
            };

            _db.AuditEntities.Add(auditEntity);
            var saved = await _db.SaveChangesAsync() > 0;

            if (!saved) return false;

            // After saving header, create audit detail rows if details is JSON representing fields
            try
            {
                if (!string.IsNullOrWhiteSpace(details))
                {
                    // Try parse as JSON object
                    using var doc = System.Text.Json.JsonDocument.Parse(details);
                    if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        var detailEntities = new List<AuditEntityKey>();

                        // If this is UPDATE for CustomerContact try to capture old values
                        Dictionary<string, string?> oldValues = new();
                        if (entityName == "CustomerContact" && action == "UPDATE")
                        {
                            // Attempt to read id from json
                            if (doc.RootElement.TryGetProperty("ContactId", out var contactIdProp))
                            {
                                var contactId = contactIdProp.GetString();
                                if (!string.IsNullOrEmpty(contactId))
                                {
                                    var existing = await _db.CustomerContacts.FindAsync(contactId);
                                    if (existing != null)
                                    {
                                        // capture existing property values as strings
                                        oldValues = existing.GetType()
                                            .GetProperties()
                                            .ToDictionary(p => p.Name, p => p.GetValue(existing)?.ToString());
                                    }
                                }
                            }
                        }

                        foreach (var prop in doc.RootElement.EnumerateObject())
                        {
                            var col = prop.Name;
                            var newVal = prop.Value.ValueKind == System.Text.Json.JsonValueKind.Null ? null : prop.Value.ToString();
                            oldValues.TryGetValue(col, out var oldVal);

                            detailEntities.Add(new AuditEntityKey
                            {
                                MtTranId = auditEntity.MtTranId,
                                ColumnName = col,
                                DisplayName = col,
                                NewVal = newVal,
                                OldVal = oldVal,
                                TableName = entityName.ToUpper(),
                                TabName = entityName
                            });
                        }

                        if (detailEntities.Count > 0)
                        {
                            _db.AuditEntityKeys.AddRange(detailEntities);
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }
            catch
            {
                // Non-fatal: if detail parsing fails, header still exists. Do not throw.
            }

            return true;
        }

        // MAKER: Hủy yêu cầu của chính mình
        public async Task<bool> CancelRequestAsync(int mtTranId, string makerId)
        {
            var request = await _db.AuditEntities.FindAsync((long)mtTranId);
            // Chỉ cho phép hủy nếu là người tạo và yêu cầu vẫn đang chờ duyệt (N)
            if (request == null || request.MtlStatus != "N" || request.Maker != makerId) return false;

            request.MtlStatus = "C"; // C: Cancelled
            request.Description = "Maker cancelled request.";

            return await _db.SaveChangesAsync() > 0;
        }

        // CHECKER: Duyệt yêu cầu
        public async Task<bool> ApproveRequestAsync(int mtTranId, string checkerId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var request = await _db.AuditEntities.FindAsync((long)mtTranId);
                if (request == null || request.MtlStatus != "N") return false;

                // Quy tắc bảo mật: Không được duyệt chính giao dịch mình tạo
                if (request.Maker == checkerId) return false;

                request.MtlStatus = "A"; // A: Đã duyệt
                request.Checker = checkerId;

                // THỰC THI THAY ĐỔI VÀO BẢNG NGHIỆP VỤ
                bool applySuccess = false;
                if (request.ObjChange == "CustomerContact")
                {
                    applySuccess = await _contactService.ApplyMakerCheckerAsync(request.MtlType ?? "", request.Description ?? "");
                }
                else
                {
                    // For other entities, we can add more logic here
                    applySuccess = true; // Temporary for entities not yet integrated
                }

                if (!applySuccess)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // CHECKER: Từ chối yêu cầu
        public async Task<bool> RejectRequestAsync(int mtTranId, string checkerId, string reason)
        {
            var request = await _db.AuditEntities.FindAsync((long)mtTranId);
            if (request == null || request.MtlStatus != "N") return false;
            if (request.Maker == checkerId) return false;

            request.MtlStatus = "R"; // R: Rejected
            request.Checker = checkerId;
            request.Description = reason;

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
