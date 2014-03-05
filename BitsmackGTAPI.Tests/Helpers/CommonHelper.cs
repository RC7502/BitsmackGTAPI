using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpVitamins;

namespace BitsmackGTAPI.Tests.Helpers
{
    public class CommonHelper
    {
        public static APIKeys CreateAPIKey(string keyName = null)
        {
            return new APIKeys()
                {
                    service_name = keyName ?? ShortGuid.NewGuid(),
                    consumer_key = ShortGuid.NewGuid(),
                    consumer_secret = ShortGuid.NewGuid(),
                    user_secret = ShortGuid.NewGuid(),
                    user_token = ShortGuid.NewGuid(),
                    last_update = DateTime.UtcNow.AddHours(-2)
                };
        }
    }
}
