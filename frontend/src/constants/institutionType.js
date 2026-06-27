// Danh sách Loại hình tổ chức (Institution Type) theo URD mục 3.2.4
// scope: 'FOREIGN' (Nước ngoài) | 'DOMESTIC' (Trong nước)
export const INSTITUTION_TYPES = [
  { id: 'FOREIGN_BRANCH_OF_A_FOREIGN_FUND_ASSET_MANAGEMENT_COMPANY', vi: 'Chi nhánh công ty quản lý quỹ (tài sản) nước ngoài', en: 'Branch of a foreign fund (asset) management company (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_BROKER_FIRM_FUTURE_COMMISION_MERCHANT', vi: 'Công ty chứng khoán / công ty kinh doanh hợp đồng tương lai nước ngoài', en: 'Broker firm / futures commission merchant (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_CLOSED_END_FUND', vi: 'Quỹ đóng nước ngoài', en: 'Closed-ended fund (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_COMMERCIAL_MERCHANT_BANK', vi: 'Ngân hàng thương mại / ngân hàng đầu tư nước ngoài', en: 'Commercial / merchant bank (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_CUSTODIAN_BANK', vi: 'Ngân hàng lưu ký nước ngoài', en: 'Custodian bank (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_DEVELOPMENT_ASSISTANCE_FUND', vi: 'Quỹ hỗ trợ phát triển nước ngoài', en: 'Development Assistance Fund (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_DISCRETIONARY_ACCOUNT_FUND', vi: 'Quỹ ủy thác đầu tư nước ngoài', en: 'Discretionary account fund (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_ETF_FUND_INDEX_FUND', vi: 'Quỹ hoán đổi danh mục (ETF) / Quỹ chỉ số nước ngoài', en: 'ETF fund / Index Fund (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_FUND_ASSET_MANAGEMENT_COMPANY', vi: 'Công ty quản lý quỹ (tài sản) nước ngoài', en: 'Fund (asset) management company (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_FUND_ESTABLISHED_AS_A_CORPORATION', vi: 'Quỹ đầu tư thành lập dưới hình thức công ty nước ngoài', en: 'Fund established as a corporation (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_INSURANCE_COMPANY', vi: 'Công ty bảo hiểm nước ngoài', en: 'Insurance company (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_INVESTMENT_BANK', vi: 'Ngân hàng đầu tư nước ngoài', en: 'Investment bank (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_MUTUAL_FUND_OPEN_ENDED_FUND', vi: 'Quỹ mở / Quỹ tương hỗ nước ngoài', en: 'Mutual fund / Open-ended fund (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_NOT_AVAILABLE', vi: 'Không xác định (tổ chức nước ngoài)', en: 'Not available (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_OTHER_FUNDS', vi: 'Quỹ khác nước ngoài', en: 'Other funds (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_OTHERS', vi: 'Tổ chức khác nước ngoài', en: 'Others (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_PENSION_FUND', vi: 'Quỹ hưu trí nước ngoài', en: 'Pension Fund (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_TRUST_FUND_A_CONTRACTUAL_FUND', vi: 'Quỹ tín thác / Quỹ hợp đồng nước ngoài', en: 'Trust fund, a contractual fund (Foreign)', scope: 'FOREIGN' },
  { id: 'FOREIGN_FUND_ESTABLISHED_AS_A_PARTNERSHIP', vi: 'Quỹ đầu tư thành lập dưới hình thức hợp danh nước ngoài', en: 'Fund established as a partnership (Foreign)', scope: 'FOREIGN' },
  { id: 'DOMESTIC_BRANCH_OF_A_DOMESTIC_FUND_ASSET_MANAGEMENT_COMPANY', vi: 'Chi nhánh công ty quản lý quỹ (tài sản) trong nước', en: 'Branch of a domestic fund (asset) management company (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_BROKER_FIRM_FUTURE_COMMISION_MERCHANT', vi: 'Công ty chứng khoán / công ty kinh doanh hợp đồng tương lai trong nước', en: 'Broker firm / futures commission merchant (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_CLOSED_END_FUND', vi: 'Quỹ đóng trong nước', en: 'Closed-ended fund (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_COMMERCIAL_MERCHANT_BANK', vi: 'Ngân hàng thương mại / ngân hàng đầu tư trong nước', en: 'Commercial / merchant bank (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_CUSTODIAN_BANK', vi: 'Ngân hàng lưu ký trong nước', en: 'Custodian bank (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_DEVELOPMENT_ASSISTANCE_FUND', vi: 'Quỹ hỗ trợ phát triển trong nước', en: 'Development Assistance Fund (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_DISCRETIONARY_ACCOUNT_FUND', vi: 'Quỹ ủy thác đầu tư trong nước', en: 'Discretionary account fund (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_ETF_FUND_INDEX_FUND', vi: 'Quỹ hoán đổi danh mục (ETF) / Quỹ chỉ số trong nước', en: 'ETF fund / Index Fund (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_FUND_ASSET_MANAGEMENT_COMPANY', vi: 'Công ty quản lý quỹ (tài sản) trong nước', en: 'Fund (asset) management company (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_FUND_ESTABLISHED_AS_A_CORPORATION', vi: 'Quỹ đầu tư thành lập dưới hình thức công ty trong nước', en: 'Fund established as a corporation (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_FUND_ESTABLISHED_AS_A_PARTNERSHIP', vi: 'Quỹ đầu tư thành lập dưới hình thức hợp danh trong nước', en: 'Fund established as a partnership (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_INSURANCE_COMPANY', vi: 'Công ty bảo hiểm trong nước', en: 'Insurance company (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_INVESTMENT_BANK', vi: 'Ngân hàng đầu tư trong nước', en: 'Investment bank (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_MUTUAL_FUND_OPEN_ENDED_FUND', vi: 'Quỹ mở / Quỹ tương hỗ trong nước', en: 'Mutual fund / Open-ended fund (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_OTHERS', vi: 'Tổ chức khác', en: 'Others', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_PENSION_FUND', vi: 'Quỹ hưu trí trong nước', en: 'Pension Fund (Domestic)', scope: 'DOMESTIC' },
  { id: 'DOMESTIC_TRUST_FUND_A_CONTRACTUAL_FUND', vi: 'Quỹ tín thác / Quỹ hợp đồng trong nước', en: 'Trust fund, a contractual fund (Domestic)', scope: 'DOMESTIC' },
  { id: 'PORTFOLIO', vi: 'Tổ chức tự doanh trong nước', en: 'Domestic proprietary trading institution', scope: 'DOMESTIC' },
  { id: 'PORTFOLIO_FR', vi: 'Tổ chức tự doanh nước ngoài', en: 'Foreign proprietary trading institution', scope: 'FOREIGN' }
]

// Lọc loại tổ chức theo phạm vi trong/ngoài nước
export function institutionTypesByScope(scope) {
  return INSTITUTION_TYPES.filter(t => t.scope === scope)
}
