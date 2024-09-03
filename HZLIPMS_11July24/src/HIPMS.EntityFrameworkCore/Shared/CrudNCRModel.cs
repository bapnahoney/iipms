using HIPMS.Models;
using System;
using System.Collections.Generic;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Shared;

public class CrudNCRModel
{
    public CrudNCRModel()
    {
        NCRDocuments = new List<NCRDocument>();
    }
    public long Id { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public ClientRoles UserRole { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Discipline Discipline { get; set; } = Discipline.Civil;//civil ,mechenical, e&I,  process
    public string NCRNo { get; set; } = string.Empty;
    public string NCRDescription { get; set; } = string.Empty;
    public DateTime DateRaised { get; set; }
    public DateTime CompletionDate { get; set; }
    public Criticality Criticality { get; set; } = Criticality.Medium;//low,high,medium
    public string VendorComments { get; set; } = string.Empty;
    public string UserRemark { get; set; } = string.Empty;
    public string OECActionRemark { get; set; } = string.Empty;//oec
    public NCRStatus Status { get; set; }
    public List<NCRDocument> NCRDocuments { get; set; }
}

