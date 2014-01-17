using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Helpers
{
    public class MathHelper
    {
        public static int Average(List<int> list)
        {
            return list.Any() ? (int) Math.Round(list.Average(), 0) : 0;
        }

        public static int TrendAverage(List<int> list)
        {
            if (list.Any())
            {
                double trend = 0;
                foreach (var item in list)
                {
                    if (trend.Equals(0))
                        trend = item;
                    else
                    {
                        trend = trend + (0.1*(item - trend));
                    }
                }

                return (int) Math.Round(trend, 0);
            }
            return 0;
        }
    }
}