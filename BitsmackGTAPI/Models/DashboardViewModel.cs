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
        public List<string> Texts;
        public List<DashboardItemViewModel> Items;
        public int Positive;

        public DashboardCategoryViewModel()
        {
            Items = new List<DashboardItemViewModel>();
            Texts = new List<string>();
        }

    }

    public class DashboardItemViewModel
    {
        public string Title;
        public List<string> Texts;
        public int Positive;

        public DashboardItemViewModel()
        {
            Texts = new List<string>();
        }
    }
}