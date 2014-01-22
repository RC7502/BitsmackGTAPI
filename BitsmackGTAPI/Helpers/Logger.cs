using System;
using BitsmackGTAPI.Constants;
using StructureMap;

namespace BitsmackGTAPI.Helpers
{
    public class Logger
    {
        public static void WriteLog(EventLogSeverity severity, string message)
        {
            var logRepo = ObjectFactory.GetInstance<GTRepository<EventLog>>();

            var log = new EventLog
                {
                    severity = (int)severity, 
                    message = message,
                    eventdate = DateTime.UtcNow
                };
            logRepo.Insert(log);
            logRepo.Save();
        }
    }
}