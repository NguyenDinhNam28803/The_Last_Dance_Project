namespace The_Last_Dance_Project.Constants
{
    /// <summary>
    /// Trạng thái giao dịch Maker-Checker lưu tại MTTRAN.MtlStatus.
    /// </summary>
    public static class TransactionStatus
    {
        public const string Pending = "N";    // Chờ duyệt
        public const string Approved = "A";   // Đã duyệt
        public const string Rejected = "R";   // Bị từ chối
        public const string Cancelled = "C";  // Maker hủy
    }

    /// <summary>
    /// Loại hành động của giao dịch Maker-Checker lưu tại MTTRAN.MtlType.
    /// Lưu ý: cột MTLTYPE giới hạn 3 ký tự nên dùng mã ngắn I/U/D.
    /// </summary>
    public static class TransactionType
    {
        public const string Insert = "I";
        public const string Update = "U";
        public const string Delete = "D";

        /// <summary>Chuẩn hóa action đầu vào (CREATE/ADD/NEW -> I, ...).</summary>
        public static string Normalize(string? action)
        {
            var a = (action ?? string.Empty).Trim().ToUpperInvariant();
            return a switch
            {
                "CREATE" or "ADD" or "NEW" or "INSERT" or "I" => Insert,
                "UPDATE" or "EDIT" or "MODIFY" or "U" => Update,
                "DELETE" or "REMOVE" or "DEL" or "D" => Delete,
                _ => a.Length > 3 ? a.Substring(0, 3) : a
            };
        }
    }

    /// <summary>
    /// Trạng thái bản ghi nghiệp vụ (Client) theo đặc tả URD.
    /// Dùng cho Customer.RecordStatus và hiển thị trên giao diện.
    /// </summary>
    public static class RecordStatus
    {
        public const string PendingInsert = "PI"; // Chờ duyệt thêm
        public const string PendingUpdate = "PU"; // Chờ duyệt sửa
        public const string PendingDelete = "PD"; // Chờ duyệt xóa
        public const string Active = "A";         // Đã duyệt
        public const string Rejected = "R";       // Từ chối
        public const string Deleted = "D";        // Đã xóa
    }
}
