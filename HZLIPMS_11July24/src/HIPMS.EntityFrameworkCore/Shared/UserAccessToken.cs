using Abp.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIPMS.Shared;  

public class UserAccessToken : UserToken
{
    public UserAccessToken()
    {
        TenantId = 1;
        UserId = 1;
        Name = string.Empty;
        Value = string.Empty;
        ExpireDate = DateTime.MinValue;
    }
}

