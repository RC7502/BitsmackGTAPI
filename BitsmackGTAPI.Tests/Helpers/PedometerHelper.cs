using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitbit.Models;

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
                    weight = weight ?? 172.2,
                    bodyfat = weight ?? 19.1
                };

        }

        public static Weight CreateWeight(List<WeightLog> weights = null)
        {
            return new Weight()
                {
                    Weights = weights ?? new List<WeightLog>()
                        {
                            CreateWeightLog(),
                            CreateWeightLog(),
                            CreateWeightLog(),
                        }
                };
        }

        public static WeightLog CreateWeightLog()
        {
            throw new NotImplementedException();
        }
    }
}
