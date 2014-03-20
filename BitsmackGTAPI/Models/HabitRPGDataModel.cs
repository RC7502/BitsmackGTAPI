using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HabitRPG.NET.Models;

namespace BitsmackGTAPI.Models
{
    public class HabitRPGDataModel
    {
        public List<Task> Tasks { get; set; }
        public List<Tag> Tags { get; set; }
    }
}