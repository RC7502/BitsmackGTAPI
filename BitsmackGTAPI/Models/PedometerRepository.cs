using System.Linq;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI.Models
{
    public class PedometerRepository : IPedometerRepository
    {
        readonly BSGTEntities _context = new BSGTEntities();
        public IQueryable<Pedometer> All 
        {
            get { return _context.Pedometer; } 
        }
    }
}