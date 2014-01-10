using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsmackGTAPI.Tests.Helpers
{
    public class PedometerHelper
    {
        public static Pedometer Create(int? id = null, 
            int? steps = null, int? sleep = null,
            DateTime? trandate = null, double? weight = null,
            double? bodyfat = null)
        {
            return new Pedometer()
                {
                    id = id ?? IdSequencer.Next(),
                    steps = steps ?? 3000,
                    sleep = sleep ?? 8*60,
                    trandate = trandate ?? DateSequencer.Next(),
                    weight = 172.2,
                    bodyfat = 19.1
                };

        }
    }
}
