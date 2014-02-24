using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Models
{
    public class BudgetCategoryViewModel
    {
        public string Name { get; set; }
        public float? Credit { get; set; }
        public float? Debit { get; set; }
        public string YearMonth { get; set; }

    }
}