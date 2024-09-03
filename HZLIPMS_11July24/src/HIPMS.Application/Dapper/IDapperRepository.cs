using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.Dapper;

public interface IDapperRepository
{
    Task<T?> GetSingle<T>(string query, DynamicParameters? sp_params, CancellationToken cancellationToken, CommandType commandType = CommandType.StoredProcedure);
    Task<List<T>> GetAll<T>(string query, DynamicParameters? sp_params, CancellationToken cancellationToken, CommandType commandType = CommandType.StoredProcedure);
    Task<List<T>?> GetAllJsonData<T>(string query, DynamicParameters? sp_params, CancellationToken cancellationToken, CommandType commandType = CommandType.StoredProcedure);

}