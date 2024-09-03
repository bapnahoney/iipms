using HIPMS.Debugging;

namespace HIPMS
{
    public class HIPMSConsts
    {
        public const string LocalizationSourceName = "HIPMS";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;
        public const int PageSize = 10;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "21efba4f5c1f4bcf9dc2e1d5d3322030";
    }
}
