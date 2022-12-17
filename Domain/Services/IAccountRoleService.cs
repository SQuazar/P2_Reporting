using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Services;

public interface IAccountRoleService : IDataService<AccountRole>
{
    Task<Account> AddRole(Account account, int roleId);
    Task<Account> AddRoles(Account account, IEnumerable<int> rolesId);
    Task<Account> RemoveRole(Account account, int roleId);
    Task<Account> RemoveRoles(Account account, IEnumerable<int> rolesId);
    Task<Account> ClearRoles(Account account);
}