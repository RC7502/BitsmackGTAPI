using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Helpers;

namespace BitsmackGTAPI.Models
{
    public class BudgetService : IBudgetService
    {
        public static readonly DateTime StartDate = new DateTime(2008, 1, 1);
        private readonly IGTRepository<Mint> _mintRepo;

        public BudgetService(IGTRepository<Mint> mintRepo)
        {
            _mintRepo = mintRepo;
        }


        public List<BudgetCategoryViewModel> GetMonthCategories()
        {
            var list = new List<BudgetCategoryViewModel>();
            var allTran = _mintRepo.AllForRead().OrderBy(x => x.Date);


            for (var counter = StartDate; counter < DateTime.Now.Date; counter = counter.AddMonths(1))
            {
                list.AddRange(allTran.Where(
                    x => x.Date != null && x.Date.Value.Year == counter.Year && x.Date.Value.Month == counter.Month)
                                .GroupBy(row => new {row.Category})
                                .Select(y => new BudgetCategoryViewModel()
                                    {
                                        Name = y.Key.Category,
                                        Credit = y.Where(z => z.Transaction_Type == "credit").Sum(z => z.Amount),
                                        Debit = y.Where(z => z.Transaction_Type == "debit").Sum(z => z.Amount),                                      
                                        YearMonth = TimeHelper.DateToMonthYear(counter)
                                    }));
            }


            return list;
        }
    }
}