using Abp.Application.Services.Dto;

namespace HIPMS.IC.Dto;

public class PagedICResultRequestDto : PagedResultRequestDto
{
    public string Keyword { get; set; }

}
