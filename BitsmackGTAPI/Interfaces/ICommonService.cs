using System.Collections.Generic;
using BitsmackGTAPI.Constants;

namespace BitsmackGTAPI.Interfaces
{
    public interface ICommonService
    {
        IEnumerable<APIKeys> GetAPIKeys();
        void UpdateAPIKey(APIKeys key);
        void WriteLog(EventLogSeverity severity, string message);
    }
}