using System.Collections.Generic;

namespace BitsmackGTAPI.Models
{
    public interface IBudgetService
    {
        List<BudgetCategoryViewModel> GetMonthCategories();
    }
}