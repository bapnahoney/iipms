using Abp.Application.Services.Dto;

namespace HIPMS.Authorization.PO.Dto;

public class PagedPOResultRequestDto : PagedResultRequestDto
{
    public string PONo { get; set; }
    public long EditId { get; set; }
}