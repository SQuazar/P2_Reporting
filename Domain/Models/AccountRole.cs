using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("account_role")]
public class AccountRole
{
    public int AccountId { get; set; }
    public Account Account { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; }
}