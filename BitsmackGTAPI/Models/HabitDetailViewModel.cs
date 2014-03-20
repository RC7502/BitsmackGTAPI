using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Models
{
    public class HabitDetailViewModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string TranDate { get; set; }
        public double? Points { get; set; }
    }
}