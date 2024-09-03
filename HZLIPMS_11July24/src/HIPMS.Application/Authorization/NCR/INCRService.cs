using HIPMS.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HIPMS.Authorization.NCR;

public interface INCRService
{

    Task<List<CrudNCRModel>> CreateAsync(CrudNCRModel input);
    Task<List<CrudNCRModel>> GetAllAsync();
    Task<CrudNCRModel> GetAsync(long id);
    Task<List<CrudNCRModel>> UpdateAsync(CrudNCRModel input);

}