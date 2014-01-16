using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Interfaces
{
    public interface IPedometerRepository
    {
        IQueryable<Pedometer> AllForRead { get; }
    }
}