using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HIPMS.Models
{
    public partial class POMaster : CreationAuditedEntity<long>
    {
        public POMaster()
        {
            POItems = new HashSet<POItem>();
            NCRs = new HashSet<NCRMaster>();
        }
        [MaxLength(50)]
        public string POType { get; set; } = string.Empty;
        public string PONo { get; set; } = string.Empty;
        public string Plant { get; set; } = string.Empty;
        public string VendorNo { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string CreatedBySAPUser { get; set; } = string.Empty;
        public string POEnvironment { get; set; } = string.Empty;
        public virtual ICollection<POItem> POItems { get; set; }
        public virtual ICollection<NCRMaster> NCRs { get; set; }
    }

    public partial class POItem : CreationAuditedEntity<long>
    {
        public long POMasterId { get; set; }
        [ForeignKey("POMasterId")]
        public string PONo { get; set; } = string.Empty;
        public string ItemNo { get; set; } = string.Empty;
        public string MaterialNo { get; set; } = string.Empty;
        public string MaterialDescription { get; set; } = string.Empty;
        public string? UOM { get; set; }
        public decimal? POQty { get; set; }
        public int MaterialClass { get; set; }
        public string ServiceNo { get; set; } = string.Empty;
        public string ServiceDescription { get; set; } = string.Empty;
        public string Approver { get; set; } = string.Empty;
        public int Status { get; set; }
        public float? ServiceQty { get; set; }
        public string? ServiceUOM { get; set; }
        public virtual POMaster POMaster { get; set; }
    }
}

