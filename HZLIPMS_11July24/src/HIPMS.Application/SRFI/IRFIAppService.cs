using HIPMS.IC.Dto;
using HIPMS.Models;
using HIPMS.Shared;
using HIPMS.SRFI.Dto;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.SRFI;

public interface IRFIAppService
{
    Task<CreateRFIRequestDto> CreateAsync(CreateRFIRequestDto input, CancellationToken cancellationToken);
    Task<List<RFI>> GetAllReqItemsAsync(PagedRFIResultRequestDto input);
    Task<POMaster> SearchPODetailAsync(SearchPORequest input, CancellationToken cancellationToken);
    Task<float> GetRFIPreviousQtyAsync(string poNo, string itemNo, string serviceNo, CancellationToken cancellationToken);
    Task<RFI> GetReqItemsAsync(long id);
    Task<RFIEditRequest> UpdateAsync(RFIEditRequest input, ClaimsPrincipal userObj, CancellationToken cancellationToken);
    Task<bool> IsValidUser();
    Task<OpenNCROnPO> GetOpenNCRCount(long POMasterId);
}