using HIPMS.IC.Dto;
using HIPMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HIPMS.DC
{
    public interface IDispatchClearanceService
    {
        Task<List<DispatchClearance>> GetAllAsync(PagedICResultRequestDto input);
    }
}
