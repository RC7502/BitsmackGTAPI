﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Models
{
    public class GoalsSummaryViewModel
    {
        public List<GoalSummaryViewModel> Items { get; set; }

        public GoalsSummaryViewModel()
        {
            Items = new List<GoalSummaryViewModel>();
        }
    }

    public class GoalSummaryViewModel
    {
    }
}