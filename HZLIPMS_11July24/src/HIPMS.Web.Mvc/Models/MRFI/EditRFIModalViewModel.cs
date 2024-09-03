
using System.Collections.Generic;


namespace HIPMS.Web.Models.MRFI;

public class EditRFIModalViewModel
{
    public EditRFIModalViewModel()
    {
        RFIItems = new List<RFIItemsData>();
    }
    public long Id { get; set; }

    public List<RFIItemsData> RFIItems { get; set; }
}


