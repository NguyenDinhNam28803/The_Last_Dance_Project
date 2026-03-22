using System.Threading.Tasks;

namespace The_Last_Dance_Project.Interfaces
{
    public interface IMakerCheckerService
    {
        Task<bool> SubmitRequestAsync(string entityName, string entityId, string action, string makerId, string details);
        Task<bool> ApproveRequestAsync(int mtTranId, string checkerId);
        Task<bool> RejectRequestAsync(int mtTranId, string checkerId, string reason);
        Task<bool> CancelRequestAsync(int mtTranId, string makerId);
    }
}
