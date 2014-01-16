using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Constants;
using StructureMap;

namespace BitsmackGTAPI.Models
{
    public class Logger
    {
        public static void WriteLog(EventLogSeverity severity, string message)
        {
            var _logRepo = ObjectFactory.GetInstance<GTRepository<EventLog>>();

            var log = new EventLog { severity = (int)severity, message = message };
            _logRepo.Insert(log);
        }
    }
}