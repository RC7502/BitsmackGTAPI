using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI.Models
{
    public class DAL : IDAL
    {
        private readonly IGTRepository<Pedometer> _pedometerRepo;
        private readonly IGTRepository<Cardio> _cardioRepo;
        private readonly IGTRepository<TimedActivities> _timedRepo;
        private readonly IGTRepository<Mint> _mintRepo;
        private readonly IGTRepository<APIKeys> _apiKeysRepo;
        private readonly IGTRepository<EventLog> _logRepo; 
 
        public DAL(IGTRepository<Pedometer> pedometerRepo, IGTRepository<Cardio> cardioRepo, 
            IGTRepository<TimedActivities> timedRepo, IGTRepository<Mint> mintRepo, IGTRepository<APIKeys> apiRepo,
            IGTRepository<EventLog> logRepo )
        {
            _pedometerRepo = pedometerRepo;
            _cardioRepo = cardioRepo;
            _timedRepo = timedRepo;
            _mintRepo = mintRepo;
            _apiKeysRepo = apiRepo;
            _logRepo = logRepo;
        }

        public IEnumerable<Pedometer> GetPedometerRecords(DateTime start, DateTime end)
        {
            return _pedometerRepo.AllForRead().Where(x => x.trandate >= start && x.trandate <= end);
        }

        public IEnumerable<Cardio> GetCardioRecords()
        {
            return _cardioRepo.AllForRead();
        }

        public void Update(Cardio existingTran)
        {
            _cardioRepo.Update(existingTran);
        }

        public void Insert(Cardio pedometer)
        {
            _cardioRepo.Insert(pedometer);
        }

        public void SaveCardio()
        {
            _cardioRepo.Save();
        }

        public IEnumerable<TimedActivities> GetTimedActivityRecords()
        {
            return _timedRepo.AllForRead();
        }

        public void Insert(TimedActivities newRec)
        {
            _timedRepo.Insert(newRec);
        }

        public void SaveTimedActivities()
        {
            _timedRepo.Save();
        }

        public IEnumerable<Mint> GetMintRecords()
        {
            return _mintRepo.AllForRead();
        }

        public IEnumerable<APIKeys> GetAPIKeys()
        {
            return _apiKeysRepo.AllForRead();
        }

        public void Update(APIKeys existingTran)
        {
            _apiKeysRepo.Update(existingTran);
        }

        public void SaveAPIKey()
        {
            _apiKeysRepo.Save();
        }

        public void Insert(EventLog newRec)
        {
            _logRepo.Insert(newRec);
        }

        public void SaveLog()
        {
            _logRepo.Save();
        }

        public IEnumerable<Pedometer> GetPedometerRecords()
        {
            return _pedometerRepo.AllForRead();
        }

        public void Update(Pedometer existingTran)
        {
            _pedometerRepo.Update(existingTran);
        }

        public void Insert(Pedometer pedometer)
        {
            _pedometerRepo.Insert(pedometer);
        }

        public void SavePedometer()
        {
            _pedometerRepo.Save();
        }
    }
}