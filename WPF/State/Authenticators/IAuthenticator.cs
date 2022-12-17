using System;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services.AuthenticationServices;

namespace WPF.State.Authenticators;

public interface IAuthenticator
{
    Account? CurrentAccount { get; set; }
    bool IsLoggedIn { get; }
    event Action StateChanged;

    Task<IAuthenticationService.RegistrationResult> Register(string username, string firstname, string surname,
        string? middleName, string password, string confirmPassword);

    Task Login(string username, string password);
    void Logout();
}