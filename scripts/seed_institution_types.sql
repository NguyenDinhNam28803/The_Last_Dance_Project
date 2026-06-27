/*
  Seed danh mục Loại hình tổ chức (Institution Type) theo URD mục 3.2.4
  Bảng: SYSTEMCODE / SYSTEMCODE_VALUE (SQL Server)
  Script idempotent: chạy lại nhiều lần không tạo bản ghi trùng.

  Cách dùng: mở SSMS/Azure Data Studio kết nối tới DB Last_Dance_API và chạy file này,
  hoặc: sqlcmd -S <server> -d Last_Dance_API -i scripts/seed_institution_types.sql
*/

SET NOCOUNT ON;

-- 1) Tạo nhóm danh mục INSTITUTION_TYPE nếu chưa có
IF NOT EXISTS (SELECT 1 FROM SYSTEMCODE WHERE SYSTEMCODEID = 'INSTITUTION_TYPE')
BEGIN
    INSERT INTO SYSTEMCODE (SYSTEMCODEID, NAME, DESCRIPTION, ISACTIVE)
    VALUES ('INSTITUTION_TYPE', N'Loại hình tổ chức', N'Danh mục loại hình tổ chức theo URD', 'Y');
END

-- 2) Chuẩn bị dữ liệu 40 giá trị
DECLARE @vals TABLE (CodeValue NVARCHAR(50), DisplayValue NVARCHAR(200), DisplayValueEn NVARCHAR(200), OrderBy INT);

INSERT INTO @vals (CodeValue, DisplayValue, DisplayValueEn, OrderBy) VALUES
('FOREIGN_BRANCH_OF_A_FOREIGN_FUND_ASSET_MANAGEMENT_COMPANY', N'Chi nhánh công ty quản lý quỹ (tài sản) nước ngoài', N'Branch of a foreign fund (asset) management company (Foreign)', 1),
('FOREIGN_BROKER_FIRM_FUTURE_COMMISION_MERCHANT', N'Công ty chứng khoán / công ty kinh doanh hợp đồng tương lai nước ngoài', N'Broker firm / futures commission merchant (Foreign)', 2),
('FOREIGN_CLOSED_END_FUND', N'Quỹ đóng nước ngoài', N'Closed-ended fund (Foreign)', 3),
('FOREIGN_COMMERCIAL_MERCHANT_BANK', N'Ngân hàng thương mại / ngân hàng đầu tư nước ngoài', N'Commercial / merchant bank (Foreign)', 4),
('FOREIGN_CUSTODIAN_BANK', N'Ngân hàng lưu ký nước ngoài', N'Custodian bank (Foreign)', 5),
('FOREIGN_DEVELOPMENT_ASSISTANCE_FUND', N'Quỹ hỗ trợ phát triển nước ngoài', N'Development Assistance Fund (Foreign)', 6),
('FOREIGN_DISCRETIONARY_ACCOUNT_FUND', N'Quỹ ủy thác đầu tư nước ngoài', N'Discretionary account fund (Foreign)', 7),
('FOREIGN_ETF_FUND_INDEX_FUND', N'Quỹ hoán đổi danh mục (ETF) / Quỹ chỉ số nước ngoài', N'ETF fund / Index Fund (Foreign)', 8),
('FOREIGN_FUND_ASSET_MANAGEMENT_COMPANY', N'Công ty quản lý quỹ (tài sản) nước ngoài', N'Fund (asset) management company (Foreign)', 9),
('FOREIGN_FUND_ESTABLISHED_AS_A_CORPORATION', N'Quỹ đầu tư thành lập dưới hình thức công ty nước ngoài', N'Fund established as a corporation (Foreign)', 10),
('FOREIGN_INSURANCE_COMPANY', N'Công ty bảo hiểm nước ngoài', N'Insurance company (Foreign)', 11),
('FOREIGN_INVESTMENT_BANK', N'Ngân hàng đầu tư nước ngoài', N'Investment bank (Foreign)', 12),
('FOREIGN_MUTUAL_FUND_OPEN_ENDED_FUND', N'Quỹ mở / Quỹ tương hỗ nước ngoài', N'Mutual fund / Open-ended fund (Foreign)', 13),
('FOREIGN_NOT_AVAILABLE', N'Không xác định (tổ chức nước ngoài)', N'Not available (Foreign)', 14),
('FOREIGN_OTHER_FUNDS', N'Quỹ khác nước ngoài', N'Other funds (Foreign)', 15),
('FOREIGN_OTHERS', N'Tổ chức khác nước ngoài', N'Others (Foreign)', 16),
('FOREIGN_PENSION_FUND', N'Quỹ hưu trí nước ngoài', N'Pension Fund (Foreign)', 17),
('FOREIGN_TRUST_FUND_A_CONTRACTUAL_FUND', N'Quỹ tín thác / Quỹ hợp đồng nước ngoài', N'Trust fund, a contractual fund (Foreign)', 18),
('FOREIGN_FUND_ESTABLISHED_AS_A_PARTNERSHIP', N'Quỹ đầu tư thành lập dưới hình thức hợp danh nước ngoài', N'Fund established as a partnership (Foreign)', 19),
('DOMESTIC_BRANCH_OF_A_DOMESTIC_FUND_ASSET_MANAGEMENT_COMPANY', N'Chi nhánh công ty quản lý quỹ (tài sản) trong nước', N'Branch of a domestic fund (asset) management company (Domestic)', 20),
('DOMESTIC_BROKER_FIRM_FUTURE_COMMISION_MERCHANT', N'Công ty chứng khoán / công ty kinh doanh hợp đồng tương lai trong nước', N'Broker firm / futures commission merchant (Domestic)', 21),
('DOMESTIC_CLOSED_END_FUND', N'Quỹ đóng trong nước', N'Closed-ended fund (Domestic)', 22),
('DOMESTIC_COMMERCIAL_MERCHANT_BANK', N'Ngân hàng thương mại / ngân hàng đầu tư trong nước', N'Commercial / merchant bank (Domestic)', 23),
('DOMESTIC_CUSTODIAN_BANK', N'Ngân hàng lưu ký trong nước', N'Custodian bank (Domestic)', 24),
('DOMESTIC_DEVELOPMENT_ASSISTANCE_FUND', N'Quỹ hỗ trợ phát triển trong nước', N'Development Assistance Fund (Domestic)', 25),
('DOMESTIC_DISCRETIONARY_ACCOUNT_FUND', N'Quỹ ủy thác đầu tư trong nước', N'Discretionary account fund (Domestic)', 26),
('DOMESTIC_ETF_FUND_INDEX_FUND', N'Quỹ hoán đổi danh mục (ETF) / Quỹ chỉ số trong nước', N'ETF fund / Index Fund (Domestic)', 27),
('DOMESTIC_FUND_ASSET_MANAGEMENT_COMPANY', N'Công ty quản lý quỹ (tài sản) trong nước', N'Fund (asset) management company (Domestic)', 28),
('DOMESTIC_FUND_ESTABLISHED_AS_A_CORPORATION', N'Quỹ đầu tư thành lập dưới hình thức công ty trong nước', N'Fund established as a corporation (Domestic)', 29),
('DOMESTIC_FUND_ESTABLISHED_AS_A_PARTNERSHIP', N'Quỹ đầu tư thành lập dưới hình thức hợp danh trong nước', N'Fund established as a partnership (Domestic)', 30),
('DOMESTIC_INSURANCE_COMPANY', N'Công ty bảo hiểm trong nước', N'Insurance company (Domestic)', 31),
('DOMESTIC_INVESTMENT_BANK', N'Ngân hàng đầu tư trong nước', N'Investment bank (Domestic)', 32),
('DOMESTIC_MUTUAL_FUND_OPEN_ENDED_FUND', N'Quỹ mở / Quỹ tương hỗ trong nước', N'Mutual fund / Open-ended fund (Domestic)', 33),
('DOMESTIC_OTHERS', N'Tổ chức khác', N'Others', 34),
('DOMESTIC_PENSION_FUND', N'Quỹ hưu trí trong nước', N'Pension Fund (Domestic)', 35),
('DOMESTIC_TRUST_FUND_A_CONTRACTUAL_FUND', N'Quỹ tín thác / Quỹ hợp đồng trong nước', N'Trust fund, a contractual fund (Domestic)', 36),
('PORTFOLIO', N'Tổ chức tự doanh trong nước', N'Domestic proprietary trading institution', 37),
('PORTFOLIO_FR', N'Tổ chức tự doanh nước ngoài', N'Foreign proprietary trading institution', 38);

-- 3) Chèn các giá trị chưa tồn tại (idempotent)
INSERT INTO SYSTEMCODE_VALUE (SYSTEMCODEID, CODEVALUE, DISPLAYVALUE, DISPLAYVALUEEN, ORDERBY, ISDEFAULT)
SELECT 'INSTITUTION_TYPE', v.CodeValue, v.DisplayValue, v.DisplayValueEn, v.OrderBy, 'N'
FROM @vals v
WHERE NOT EXISTS (
    SELECT 1 FROM SYSTEMCODE_VALUE s
    WHERE s.SYSTEMCODEID = 'INSTITUTION_TYPE' AND s.CODEVALUE = v.CodeValue
);

PRINT 'Seed INSTITUTION_TYPE hoàn tất.';
