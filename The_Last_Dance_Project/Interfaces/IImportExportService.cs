using The_Last_Dance_Project.Dtos;

namespace The_Last_Dance_Project.Interfaces
{
    public interface IImportExportService
    {
        /// <summary>
        /// Xuất danh sách khách hàng ra file Excel (.xlsx).
        /// </summary>
        Task<byte[]> ExportCustomersAsync();

        /// <summary>
        /// Nhập danh sách khách hàng từ file Excel.
        /// Thực hiện validate và chèn mới vào DB.
        /// </summary>
        Task<ImportResultDto> ImportCustomersAsync(Stream fileStream, string importedBy);

        /// <summary>
        /// Tạo file Excel mẫu (template) để người dùng tải về và điền thông tin.
        /// </summary>
        byte[] GenerateTemplate();
    }
}
