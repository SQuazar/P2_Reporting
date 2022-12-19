using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services;

public interface IPaginatedService<T> : IDataService<T>
{
    Task<IEnumerable<T>> GetPage(int page, int size);

    Task<int> GetPagesCount(int size);
}