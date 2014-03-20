using System.Collections.Generic;
using System.Linq;

namespace BitsmackGTAPI
{
    public interface IGTRepository<T> where T : class
    {
        IQueryable<T> AllForRead();
        void Update(T key);
        void Insert(T log);
        void Delete(T existingTran);
        void Save();
    }
}