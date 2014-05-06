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
        public List<DashboardItemViewModel> Items;
        public string Name;

        public DashboardCategoryViewModel()
        {
            Items = new List<DashboardItemViewModel>();
        }

    }

    public class DashboardItemViewModel
    {
        public string Status;
        public int BarActual;
        public int BarMax;
        public int BarMin;
        public string ItemType;
        public string Text;
    }
}