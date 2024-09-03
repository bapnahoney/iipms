using HIPMS.Models;
using System;
using System.Collections.Generic;

namespace HIPMS.DC.Dto;

public class CreateDCRequestDto
{
    public CreateDCRequestDto()
    {
        // DCData = new HashSet<DCData>();
        // DCDocuments = new HashSet<DCDocument>();
    }

    public long POMasterId { get; set; }

    public string VendorNo { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string DispatchNo { get; set; } = string.Empty;
    public string DispatchMode { get; set; } = string.Empty;
    public string VendorRemark { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ManufacturerName { get; set; } = string.Empty;
    public string ManufacturerPlantAddress { get; set; } = string.Empty;
    public int Status { get; set; }
    public int SessionID { get; set; }
    public int SyncStatus { get; set; }
    public DateTime SyncOn { get; set; }
    public int SyncCount { get; set; }
    public List<DCData> DCData { get; set; }
    public POMaster POMaster { get; set; }
    public List<DCDocument> DCDocuments { get; set; }

}
