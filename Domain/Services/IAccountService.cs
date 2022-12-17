using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Services;

public interface IAccountService : IDataService<Account>
{
    Task<Account> GetByUsername(string username);
}