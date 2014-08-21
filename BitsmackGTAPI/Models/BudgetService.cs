using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Helpers;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI.Models
{
    public class BudgetService : IBudgetService
    {
        public static readonly DateTime StartDate = new DateTime(2008, 1, 1);
        public readonly IDAL _dal;

        public BudgetService(IDAL dal)
        {
            _dal = dal;
        }


        public List<BudgetCategoryViewModel> GetMonthCategories()
        {
            var list = new List<BudgetCategoryViewModel>();
            var allTran = _dal.GetMintRecords().OrderBy(x => x.Date);


            for (var counter = StartDate; counter < DateTime.Now.Date; counter = counter.AddMonths(1))
            {
                list.AddRange(allTran.Where(
                    x => x.Date != null && x.Date.Value.Year == counter.Year && x.Date.Value.Month == counter.Month)
                                .GroupBy(row => new {row.Category})
                                .Select(y => new BudgetCategoryViewModel()
                                    {
                                        Name = y.Key.Category,
                                        Credit = y.Where(z => z.TransactionType == "credit").Sum(z => z.Amount),
                                        Debit = y.Where(z => z.TransactionType == "debit").Sum(z => z.Amount),                                      
                                        YearMonth = TimeHelper.DateToMonthYear(counter)
                                    }));
            }


            return list;
        }
    }
}