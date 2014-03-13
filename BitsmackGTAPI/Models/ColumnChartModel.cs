using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Models
{
    public class ColumnChartModel
    {
        public string[] Categories { get; set; }
        public double PlotLine { get; set; }
        public double[] SeriesData { get; set; }
    }
}