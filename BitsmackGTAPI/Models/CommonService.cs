using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI.Models
{
    public class CommonService : ICommonService
    {
        private readonly IGTRepository<APIKeys> _apiKeysRepo;
        private readonly IGTRepository<EventLog> _logRepo; 

        public CommonService(IGTRepository<APIKeys> api, IGTRepository<EventLog> log)
        {
            _apiKeysRepo = api;
            _logRepo = log;
        }

        public IEnumerable<APIKeys> GetAPIKeys()
        {
            return _apiKeysRepo.AllForRead();
        }

        public void UpdateAPIKey(APIKeys key)
        {
            _apiKeysRepo.Update(key);
            _apiKeysRepo.Save();
        }

    }
}