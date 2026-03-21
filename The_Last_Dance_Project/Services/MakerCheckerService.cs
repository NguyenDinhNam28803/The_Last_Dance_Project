using System;
using System.Threading.Tasks;
using The_Last_Dance_Project.Data;
using The_Last_Dance_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace The_Last_Dance_Project.Services
{
    public interface IMakerCheckerService
    {
        Task<bool> SubmitRequestAsync(string entityName, string entityId, string action, string makerId, string details);
        Task<bool> ApproveRequestAsync(int mtTranId, string checkerId);
        Task<bool> RejectRequestAsync(int mtTranId, string checkerId, string reason);
    }

    public class MakerCheckerService : IMakerCheckerService
    {
        private readonly ApplicationDbContext _db;

        public MakerCheckerService(ApplicationDbContext db)
        {
            _db = db;
        }

        // MAKER: Tạo yêu cầu mới
        public async Task<bool> SubmitRequestAsync(string entityName, string entityId, string action, string makerId, string details)
        {
            var auditEntity = new Auditentity
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

            _db.Auditentities.Add(auditEntity);
            return await _db.SaveChangesAsync() > 0;
        }

        // CHECKER: Duyệt yêu cầu
        public async Task<bool> ApproveRequestAsync(int mtTranId, string checkerId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var request = await _db.Auditentities.FindAsync((long)mtTranId);
                if (request == null || request.MtlStatus != "N") return false;

                // Quy tắc bảo mật: Không được duyệt chính giao dịch mình tạo
                if (request.Maker == checkerId) return false;

                request.MtlStatus = "A"; // A: Đã duyệt
                request.Checker = checkerId;

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
            var request = await _db.Auditentities.FindAsync((long)mtTranId);
            if (request == null || request.MtlStatus != "N") return false;
            if (request.Maker == checkerId) return false;

            request.MtlStatus = "R"; // R: Rejected
            request.Checker = checkerId;
            request.Description = reason;

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
