using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Interfaces
{
    public interface IDAL
    {
        IEnumerable<Pedometer> GetPedometerRecords();
        void Update(Pedometer existingTran);
        void Insert(Pedometer pedometer);
        void SavePedometer();
        IEnumerable<Pedometer> GetPedometerRecords(DateTime start, DateTime end);
        IEnumerable<Cardio> GetCardioRecords();
        void Update(Cardio existingTran);
        void Insert(Cardio pedometer);
        void SaveCardio();
        IEnumerable<TimedActivities> GetTimedActivityRecords();
        void Insert(TimedActivities newRec);
        void SaveTimedActivities();
        IEnumerable<Mint> GetMintRecords();
        IEnumerable<APIKeys> GetAPIKeys();
        void Update(APIKeys existingTran);
        void SaveAPIKey();
        void Insert(EventLog newRec);
        void SaveLog();
        IEnumerable<Todos> GetTodoRecords();
        void Update(Todos existingTran);
        void Insert(Todos todoDay);
        void SaveTodos();
    }
}