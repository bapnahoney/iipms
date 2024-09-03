using HIPMS.Models;
using System;
using System.Collections.Generic;


namespace HIPMS.IC.Dto
{
    public class CreateICRequestDto
    {
        public CreateICRequestDto()
        {
            ICItems = new List<ICData>();
        }
        public long POMasterId { get; set; }
        public string PO { get; set; } = string.Empty;
        public float POQty { get; set; }
        //public float ICPreviousQty { get; set; }
        //public float ICBalanceQty { get; set; }
        //public float ICInputQty { get; set; }
        public string VendorNo { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string VendorRemark { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ManufacturerName { get; set; } = string.Empty;
        public string ManufacturerPlantAddress { get; set; } = string.Empty;
        public string InspectionBy { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public int Status { get; set; }
        public int SessionID { get; set; }
        public int SyncStatus { get; set; }
        public DateTime SyncOn { get; set; }
        public int SyncCount { get; set; }
        public List<ICData> ICItems { get; set; }
    }
}
