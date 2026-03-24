// Users mock
export const mockUsers = [
  { custId: '1', username: 'admin', name: 'System Admin', email: 'admin@navi.vn', roleName: 'Admin', status: 'Active' },
  { custId: '2', username: 'manager01', name: 'Ngo Quoc Dat', email: 'datnq@navi.vn', roleName: 'Manager', status: 'Active' },
  { custId: '3', username: 'user01', name: 'Le Van A', email: 'vana@navi.vn', roleName: 'User', status: 'Inactive' }
]

// Clients mock - BO System Specific
export const mockClients = [
  {
    clientId: '600001',
    name: 'Công ty Cổ phần Xây dựng Hòa Bình',
    nameOther: 'Hoa Binh Construction Group',
    registrationType: 'LOCAL_INSTITUTION',
    idType: 'BUSINESS_REG',
    idNumber: '0300445566',
    idIssueDate: '2010-05-20',
    idExpiryDate: null,
    idIssuePlace: 'Sở Kế hoạch Đầu tư TPHCM',
    nationality: 'VN',
    institutionType: 'OTHER',
    gender: 'O',
    dob: null,
    fatca: false,
    custodyId: '003C600001',
    status: 'Active',
    contacts: [
      { contactType: 'ADDRESS', infoType: 'PERMANENT', detailInfo: '235 Võ Thị Sáu, P.Võ Thị Sáu, Q.3, TPHCM', isDefault: true },
      { contactType: 'PHONE', infoType: 'OFFICE', detailInfo: '02839325030', isDefault: true }
    ]
  },
  {
    clientId: '600002',
    name: 'Nguyễn Văn Nam',
    nameOther: '',
    registrationType: 'LOCAL_RETAIL',
    idType: 'CCCD',
    idNumber: '001090123456',
    idIssueDate: '2021-10-15',
    idExpiryDate: '2041-10-15',
    idIssuePlace: 'Cục Cảnh sát QLHC về TTXH',
    nationality: 'VN',
    institutionType: '',
    gender: 'M',
    dob: '1990-05-12',
    fatca: false,
    custodyId: '003C600002',
    status: 'Pending Insert',
    contacts: [
      { contactType: 'ADDRESS', infoType: 'CONTACT', detailInfo: '12 Thái Hà, Đống Đa, Hà Nội', isDefault: true },
      { contactType: 'PHONE', infoType: 'MOBILE', detailInfo: '0901234567', isDefault: true },
      { contactType: 'EMAIL', infoType: 'DEFAULT', detailInfo: 'nam.nv@gmail.com', isDefault: true }
    ]
  },
  {
    clientId: '600003',
    name: 'John Smith',
    nameOther: 'Johnathan Smith',
    registrationType: 'FOREIGN_RETAIL',
    idType: 'PASSPORT',
    idNumber: 'US123456789',
    idIssueDate: '2018-02-10',
    idExpiryDate: '2028-02-09',
    idIssuePlace: 'US EMBASSY',
    nationality: 'US',
    institutionType: '',
    gender: 'M',
    dob: '1985-11-20',
    fatca: true,
    custodyId: '003F600003',
    status: 'Active',
    contacts: [
      { contactType: 'ADDRESS', infoType: 'PERMANENT', detailInfo: '1 NY Street, New York, USA', isDefault: true },
      { contactType: 'ADDRESS', infoType: 'CONTACT', detailInfo: 'Crescent Mall, Q7, TPHCM', isDefault: false },
      { contactType: 'PHONE', infoType: 'MOBILE', detailInfo: '0988776655', isDefault: true }
    ]
  }
]

export const mockSystemCodes = [
  {
    id: 'ID_TYPE',
    name: 'Loại ĐKSH',
    description: 'Danh mục loại giấy tờ định danh',
    isActive: true,
    values: [
      { code: 'CMND', value: 'Chứng minh nhân dân', order: 1, isActive: true },
      { code: 'CCCD', value: 'Căn cước công dân', order: 2, isActive: true }
    ]
  }
]

export const mockAuditLogs = [
  { id: 1, busDate: '2024-03-24', objChange: 'Khách hàng', mtlType: 'ADD', maker: 'vuongnv', checker: '', mtlStatus: 'Pending Insert' },
  { id: 2, busDate: '2024-03-23', objChange: 'Người dùng', mtlType: 'UPDATE', maker: 'admin', checker: 'manager01', mtlStatus: 'Active', oldVal: '{"status":"Inactive"}', newVal: '{"status":"Active"}' }
]

export const mockMakerCheckerRequests = [
  { mkckId: 'REQ-001', tableName: 'Khách hàng', actionType: 'INSERT', recordName: 'Nguyễn Văn Nam', makerId: 'user01', createdAt: '2024-03-24 10:00:00', status: 'Pending' }
]
