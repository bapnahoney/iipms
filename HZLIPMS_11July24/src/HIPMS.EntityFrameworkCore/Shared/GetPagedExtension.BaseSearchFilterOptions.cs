using System.Collections.Generic;

namespace HIPMS.Shared;

public static partial class GetPagedExtension
{
    public class BaseSearchFilterOptions
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public List<string> Sort { get; set; } = new();
        public string SearchParam { get; set; } = string.Empty;
    }
}
