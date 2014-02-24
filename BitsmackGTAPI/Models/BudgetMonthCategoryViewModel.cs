using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Models
{
    public class BudgetMonthViewModel
    {
        public string YearMonth { get; set; }
        public double TotalIncome { get; set; }
        public double TotalExpenses { get; set; }
        public List<BudgetCategoryViewModel> Categories { get; set; } 

    }


}