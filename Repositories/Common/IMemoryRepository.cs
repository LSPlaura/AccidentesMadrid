using AccidentesMadrid.Models;

namespace AccidentesMadrid.Repositories.Common;

public interface IMemoryRepository<T>
{
    IEnumerable<Accidente> GetAll();
    void AddRange(IEnumerable<T> items);
}