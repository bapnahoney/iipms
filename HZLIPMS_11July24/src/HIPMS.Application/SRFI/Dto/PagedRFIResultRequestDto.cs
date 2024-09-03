using Abp.Application.Services.Dto;

namespace HIPMS.SRFI.Dto
{
    public class PagedRFIResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }

}

