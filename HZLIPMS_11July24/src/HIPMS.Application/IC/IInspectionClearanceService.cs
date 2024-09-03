using HIPMS.IC.Dto;
using HIPMS.Models;
using HIPMS.Shared;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.IC;


public interface IInspectionClearanceService
{
    Task<CreateICRequestDto> CreateAsync(CreateICRequestDto input, CancellationToken cancellationToken);
    Task<List<InspectionClearance>> GetAllReqItemsAsync(PagedICResultRequestDto input);
    Task<POMaster> SearchPODetailAsync(SearchPORequest input, CancellationToken cancellationToken);
    Task<float> GetICPreviousQtyAsync(string poNo, string itemNo, CancellationToken cancellationToken);
    Task<InspectionClearance> GetReqItemsAsync(long id);
    Task<ICEditRequest> UpdateAsync(ICEditRequest input, ClaimsPrincipal userObj, CancellationToken cancellationToken);
    Task<bool> IsValidUser();

}