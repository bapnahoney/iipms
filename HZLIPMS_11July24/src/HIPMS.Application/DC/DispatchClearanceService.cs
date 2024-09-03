using Abp.Runtime.Session;
using HIPMS.Authorization.PO;

using HIPMS.EntityFrameworkCore;
using HIPMS.IC.Dto;
using HIPMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HIPMS.DC;

public class DispatchClearanceService : IDispatchClearanceService
{
    private readonly HIPMSDbContext _db;
    private readonly IPOService _iPOService;
    private readonly IAbpSession _abpSession;

    public DispatchClearanceService(HIPMSDbContext db, IPOService iPOService, IAbpSession abpSession)
    {
        _db = db;
        _iPOService = iPOService;
        _abpSession = abpSession;
    }
    public async Task<List<DispatchClearance>> GetAllAsync(PagedICResultRequestDto input)
    {
        var resObj = await _db.DispatchClearance.Include(y => y.DCData)
                              .ThenInclude(p => p.POMaster)
                              .Where(x => x.Id > 0).ToListAsync();
        return resObj;
    }
}
