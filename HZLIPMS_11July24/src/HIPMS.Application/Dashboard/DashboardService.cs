using Dapper;
using HIPMS.Authorization.PO;
using HIPMS.Dapper;
using HIPMS.Dashboard.Dto;
using HIPMS.EntityFrameworkCore;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly HIPMSDbContext _db;
    private readonly IDapperRepository _dapper;
    private readonly IPOService _iPOService;


    public DashboardService(IDapperRepository dapper, HIPMSDbContext db, IPOService iPOService)
    {
        _db = db;
        _dapper = dapper;
        _iPOService = iPOService;
    }
    public async Task<DashboardStatisticsSPResDto> GetAllAsync(CancellationToken cancellationToken)
    {

        var spParams = new DynamicParameters();
        spParams.Add("@UserId", 0, DbType.Int32);

        var result = await _dapper.GetSingle<DashboardStatisticsSPResDto>("[dbo].[GetDashboardStatistics]"
                    , spParams, cancellationToken,
        commandType: CommandType.StoredProcedure);

        if (result == null)
        {
            //  return _response.Error("Customers enhance your stay category could not be found", AppStatusCodeError.Gone410);
        }
        return result;

    }

}
