using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsmackGTAPI.Interfaces
{
    public interface ITodoService
    {
        void RefreshData(bool overwrite, DateTime startdate, DateTime enddate);
    }
}
