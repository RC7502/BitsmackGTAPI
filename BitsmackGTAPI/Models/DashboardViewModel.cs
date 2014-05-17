using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Constants;

namespace BitsmackGTAPI.Models
{
    public class DashboardViewModel
    {
        public List<DashboardCategoryViewModel> Categories;
        public DashboardViewModel()
        {
            Categories = new List<DashboardCategoryViewModel>();
        }
    }

    public class DashboardCategoryViewModel
    {
        public string ID;
        public string Title;
        public string Text;
        public List<DashboardItemViewModel> Items;
        
        public DashboardCategoryViewModel()
        {
            Items = new List<DashboardItemViewModel>();
        }

    }

    public class DashboardItemViewModel
    {
        public string Title;
        public string Text;
    }
}