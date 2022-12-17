using System;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services.AuthenticationServices;
using WPF.State.Accounts;

namespace WPF.State.Authenticators;

public class Authenticator : IAuthenticator
{
    private readonly IAccountStore _accountStore;
    private readonly IAuthenticationService _authenticationService;

    public Account? CurrentAccount
    {
        get => _accountStore.CurrentAccount;
        set
        {
            _accountStore.CurrentAccount = value;
            StateChanged?.Invoke();
        }
    }

    public bool IsLoggedIn => CurrentAccount != null;

    public event Action? StateChanged;

    public Authenticator(IAccountStore accountStore, IAuthenticationService authenticationService)
    {
        _accountStore = accountStore;
        _authenticationService = authenticationService;
    }

    public async Task<IAuthenticationService.RegistrationResult> Register(string username, string firstname,
        string surname, string? middleName, string password, string confirmPassword)
    {
        return await _authenticationService.Register(username, firstname, surname, middleName, 
            password, confirmPassword);
    }

    public async Task Login(string username, string password)
    {
        CurrentAccount = await _authenticationService.Login(username, password);
    }

    public void Logout()
    {
        CurrentAccount = null;
    }
}