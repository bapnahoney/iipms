using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.Dapper;




public class DapperRepository : IDapperRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DapperRepository> _logger;


    public DapperRepository(IConfiguration configuration, ILogger<DapperRepository> logger)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<List<T>> GetAll<T>(string query, DynamicParameters? sp_params, CancellationToken cancellationToken, CommandType commandType = CommandType.StoredProcedure)
    {
        List<T> result = new List<T>();

        using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            try
            {
                result = (List<T>)await dbConnection.QueryAsync<T>(query, sp_params, commandType: commandType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Get Data.");

            }
        };

        return result;
    }

    public async Task<T?> GetSingle<T>(string query, DynamicParameters? sp_params, CancellationToken cancellationToken, CommandType commandType = CommandType.StoredProcedure)
    {

        T? result = default(T);

        using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();
            try
            {
                var spResult = await dbConnection.QueryAsync<T>(query, sp_params, commandType: commandType, transaction: null);
                if (spResult != null)
                    result = spResult.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Get Data.");

            }
        };

        return result;
    }

    public async Task<List<T>?> GetAllJsonData<T>(string query, DynamicParameters? sp_params, CancellationToken cancellationToken, CommandType commandType = CommandType.StoredProcedure)
    {
        List<T>? result = new();

        using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            try
            {
                var jsonDataResult = await dbConnection.QueryFirstAsync<string>(query, sp_params, commandType: commandType);
                if (!string.IsNullOrEmpty(jsonDataResult))
                    result = System.Text.Json.JsonSerializer.Deserialize<List<T>>(jsonDataResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Get Data.");

            }
        };

        return result;
    }
}

