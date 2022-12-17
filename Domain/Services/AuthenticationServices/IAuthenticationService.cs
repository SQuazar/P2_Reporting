#nullable enable
using System.Threading.Tasks;
using Domain.Exceptions;
using Domain.Models;

namespace Domain.Services.AuthenticationServices;

public interface IAuthenticationService
{
    public enum RegistrationResult
    {
        Success,
        PasswordDoNotMatch,
        UserAlreadyExists
    }

    Task<RegistrationResult> Register(string username, string firstname, string surname, string? middleName, string password, string confirmPassword);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <exception cref="UserNotFoundException">Throws if user does not exists</exception>
    /// <exception cref="InvalidPasswordException">Throws if the login fails</exception>
    /// <returns></returns>
    Task<Account> Login(string username, string password);
}