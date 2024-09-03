using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HIPMS.Models;


public partial class NCRMaster : CreationAuditedEntity<long>
{
    public NCRMaster()
    {
        NCRDocument = new HashSet<NCRDocument>();
    }

    public long POMasterId { get; set; }
    [ForeignKey("POMasterId")]
    public string PONumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Discipline { get; set; }//civil ,mechenical, e&I,  process
    public string NCRNo { get; set; } = string.Empty;
    public string NCRDescription { get; set; } = string.Empty;
    public DateTime DateRaised { get; set; }
    public DateTime CompletionDate { get; set; }
    public int Criticality { get; set; }//low,high,medium
    public string VendorComments { get; set; } = string.Empty;
    public int Status { get; set; }
    public string UserRemark { get; set; } = string.Empty;
    public string OECActionRemark { get; set; } = string.Empty;//oec
    public virtual ICollection<NCRDocument> NCRDocument { get; set; }
    public virtual POMaster POMaster { get; set; }

}
public partial class NCRDocument : CreationAuditedEntity<long>
{
    public long NCRMasterId { get; set; }
    [ForeignKey("NCRMasterId")]
    public int DocumentType { get; set; }
    public string? DocumentLocation { get; set; }
    public virtual NCRMaster NCRMaster { get; set; }
}