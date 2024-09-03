using System.Collections.Generic;

namespace HIPMS.File.Dto;


public class FileRequest
{
    public int Module { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public long ModuleRequestId { get; set; }//IC ,RFI ,NCR requestId
    public long RequestedItemId { get; set; }//IC ,RFI request item iD
    public List<ItemDocuments> ItemDocuments { get; set; } = new();

}
public class ItemDocuments
{
    public long Id { get; set; }
    public string DocumentLocation { get; set; }
}