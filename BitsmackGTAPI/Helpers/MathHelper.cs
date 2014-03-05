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
            //only average records that are not 0
            return list.Any() ? (int) Math.Round(list.Where(x=>x>0).Average(), 0) : 0;
        }

        public static int TrendAverage(List<int> list)
        {
            //only average records that are not 0
            if (list.Any())
            {
                double trend = 0;
                foreach (var item in list)
                {
                    if (item > 0)
                    {
                        if (trend.Equals(0))
                            trend = item;
                        else
                        {
                            trend = trend + (0.1*(item - trend));
                        }
                    }
                }

                return (int) Math.Round(trend, 0);
            }
            return 0;
        }

        public static double MetersToMiles(double average)
        {
            return Math.Round(average/1609.34, 2);
        }

        public static decimal TrendAverage(List<decimal> list, int decimals)
        {
            //only average records that are not 0
            if (list.Any())
            {
                decimal trend = 0;
                foreach (var item in list)
                {

                    if (item > 0)
                    {
                        if (trend.Equals(0))
                            trend = item;
                        else
                        {
                            trend = trend + (0.1m * (item - trend));
                        }
                    }
                }

                return Math.Round(trend, decimals);
            }
            return 0;
        }

        public static double Adj5KPace(double time, double distance)
        {
            return time*(Math.Pow(3.1/MetersToMiles(distance), 1.06));
        }
    }
}