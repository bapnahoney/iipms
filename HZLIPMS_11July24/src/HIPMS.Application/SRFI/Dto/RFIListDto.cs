using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;

namespace HIPMS.SRFI.Dto
{
    public class RFIListDto : EntityDto, IHasCreationTime
    {
        public string VendorNo { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string VendorRemark { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public int Status { get; set; }
        public bool IsStatic { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreationTime { get; set; }
    }

}

