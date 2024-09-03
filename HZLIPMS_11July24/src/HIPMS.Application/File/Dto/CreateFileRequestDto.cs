using System.Collections.Generic;

namespace HIPMS.File.Dto;

public class CreateFileRequestDto
{
    //public CreateFileRequestDto()
    //{

    //}
    public int Module { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public int ModuleRequestId { get; set; }//IC RFI requestId
    public int RequestedItemId { get; set; }//IC RFI request item iD
    public List<string> DocumentLocation { get; set; } = new();

}
