using System;
using System.Collections.Generic;


namespace HIPMS.SRFI.Dto
{
    internal class AddEditRFIRequest
    {
        public AddEditRFIRequest()
        {
            icItems = new List<RFIItems>();
        }
        public long id { get; set; } = 0;
        public string po { get; set; }
        public long poMasterId { get; set; }
        public string poType { get; set; } = string.Empty;
        public string vendorNo { get; set; }
        public string vendorName { get; set; }
        public string vendorRemark { get; set; }
        public string projectName { get; set; }
        //public string manufacturerName { get; set; }
        //public string manufacturerPlantAddress { get; set; }
        public List<RFIItems> icItems { get; set; }

    }
    public class RFIItems
    {
        public string po { get; set; }
        public long inspectionClearanceId { get; set; }
        public string itemNo { get; set; }
        //public string materialNo { get; set; }
        public string materialDescription { get; set; }
        public string poQty { get; set; }
        public float previousQty { get; set; }
        public float balanceQty { get; set; }
        public float inputQty { get; set; }//this qty should be less than balance qty
        public long inspectionById { get; set; }
        public string? inspectionBy { get; set; }//email
        public string? inspectionSummary { get; set; }
        public int? status { get; set; }
        public string? statusValue { get; set; }
        public int? materialClass { get; set; }
        public string? materialClassValue { get; set; }
        public string? serviceNo { get; set; }
        public string? uom { get; set; }
        public string? serviceDescription { get; set; }
        public decimal? quantity { get; set; }
        public string? approver { get; set; }
        public long approverById { get; set; }
        public DateTime? approvedOn { get; set; }
        public string? rejecter { get; set; }
        public DateTime? rejectedOn { get; set; }
        public string? referrer { get; set; }
        public DateTime? referedOn { get; set; }
        public DateTime? inspectionDoneOn { get; set; }
    }


}



