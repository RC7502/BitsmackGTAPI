using System.Collections.Generic;
using BitsmackGTAPI.Constants;

namespace BitsmackGTAPI.Interfaces
{
    public interface ICommonService
    {
        IEnumerable<APIKeys> GetAPIKeys();
        void UpdateAPIKey(APIKeys key);
        APIKeys GetAPIKeyByName(string name);
        void WriteLog(EventLogSeverity error, string name, string message);
        void LogActivity(Cardio cardio);
        void LogActivity(TimedActivities newRec);
        void LogActivity(Pedometer newRec);
        void LogActivity(Todos todoDay);
    }
}