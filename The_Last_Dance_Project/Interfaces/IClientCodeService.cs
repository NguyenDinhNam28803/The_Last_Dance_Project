using System.Threading.Tasks;

namespace The_Last_Dance_Project.Interfaces
{
    /// <summary>
    /// Các quy tắc sinh mã &amp; nghiệp vụ cho phân hệ Khách hàng (Client) theo URD.
    /// </summary>
    public interface IClientCodeService
    {
        /// <summary>Sinh ClientID 6 chữ số nhỏ nhất chưa tồn tại trong hệ thống (icon #).</summary>
        Task<string> GenerateClientIdAsync();

        /// <summary>Sinh Số TK lưu ký: 003C + ClientID (trong nước) hoặc 003F + ClientID (nước ngoài).</summary>
        string GenerateCustodyId(string registrationType, string clientId);

        /// <summary>Phát hiện dấu hiệu FATCA (Hoa Kỳ) theo quốc tịch &amp; địa chỉ.</summary>
        bool DetectFatca(string? nationality, params string?[] addresses);

        /// <summary>Kiểm tra khách hàng cá nhân đã đủ 18 tuổi theo ngày sinh (dd/MM/yyyy hoặc ISO).</summary>
        bool IsAtLeast18(string? dateOfBirth);
    }
}
