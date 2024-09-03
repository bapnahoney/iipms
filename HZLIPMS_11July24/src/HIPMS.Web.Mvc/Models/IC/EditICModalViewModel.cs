using System.Collections.Generic;

namespace HIPMS.Web.Models.IC;

public class EditRFIModalViewModel
{
    public EditRFIModalViewModel()
    {
        ICItems = new List<ICItemsData>();
    }
    public long Id { get; set; }

    public List<ICItemsData> ICItems { get; set; }
}


