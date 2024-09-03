using HIPMS.Dashboard.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace HIPMS.Dashboard;

public interface IDashboardService
{
    Task<DashboardStatisticsSPResDto> GetAllAsync(CancellationToken cancellationToken);
}
