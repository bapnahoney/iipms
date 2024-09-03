using HIPMS.Authorization.PO.Dto;
using HIPMS.IC.Dto;
using HIPMS.Models;
using HIPMS.Shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.Authorization.PO;

public interface IPOService
{
    //Task<CreateICRequestDto> CreateAsync(CreateICRequestDto input);
    Task<List<POMaster>> GetAllAsync(PagedPOResultRequestDto input);
    //Task<POMaster> GetPODetailAsync(POResultRequestDto input);
    Task<POMaster> GetPODetailAsync(POResultRequestDto input);
    //Task<List<POMaster>> GetPODetailFromSAPAsync(POResultRequestDto input);
    Task<object> GetPODetailFromSAPAsync(PagedPOResultRequestDto input, CancellationToken cancellationToken);
    Task<CrudUserPOMap> GetUserPODDAsync();
    Task<List<ViewUserPOMap>> UserPOMapUpdateAsync(CrudUserPOMap reqObj);
    Task<List<ViewUserPOMap>> GetAllMappingAsync();
    Task<POMaster> SearchPODetailAsync(SearchPORequest input, CancellationToken cancellationToken);
    Task<object> GetPODetailFromSAPV2Async(PagedPOResultRequestDto input, CancellationToken cancellationToken);
    Task<object> GetPODetailFromSAPV2OnApproveAsync(PagedPOResultRequestDto input, CancellationToken cancellationToken);
}