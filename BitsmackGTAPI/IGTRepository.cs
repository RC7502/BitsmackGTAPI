using System.Collections.Generic;

namespace BitsmackGTAPI
{
    public interface IGTRepository<T> where T : class
    {
        IEnumerable<T> AllForRead();
        void Update(T key);
        void Insert(T log);
        void Delete(T existingTran);
        void Save();
    }
}