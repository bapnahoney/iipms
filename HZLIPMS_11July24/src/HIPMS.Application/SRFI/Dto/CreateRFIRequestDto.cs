using HIPMS.Models;
using System;
using System.Collections.Generic;


namespace HIPMS.SRFI.Dto;

public class CreateRFIRequestDto
{
    public CreateRFIRequestDto()
    {
        RFIItems = new List<RFIData>();
    }
    public long POMasterId { get; set; }
    public string PONo { get; set; } = string.Empty;
    public float POQty { get; set; }
    public string VendorNo { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorRemark { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    //public string ManufacturerName { get; set; } = string.Empty;
    //public string ManufacturerPlantAddress { get; set; } = string.Empty;
    public string InspectionBy { get; set; } = string.Empty;
    public int Status { get; set; }
    public int SessionID { get; set; }
    public int SyncStatus { get; set; }
    public DateTime SyncOn { get; set; }
    public int SyncCount { get; set; }
    public List<RFIData> RFIItems { get; set; }
}
