#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNet.Identity;

namespace Domain.Services.AuthenticationServices;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAccountService _accountService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthenticationService(IAccountService accountService, IPasswordHasher passwordHasher)
    {
        _accountService = accountService;
        _passwordHasher = passwordHasher;
    }

    public async Task<IAuthenticationService.RegistrationResult> Register(string username, string firstname,
        string surname, string? middleName, string password, string confirmPassword)
    {
        if (password != confirmPassword)
            return IAuthenticationService.RegistrationResult.PasswordDoNotMatch;
        var find = await _accountService.GetByUsername(username);
        if (find != null)
            return IAuthenticationService.RegistrationResult.UserAlreadyExists;
        var hashedPassword = _passwordHasher.HashPassword(password);
        var account = new Account
        {
            Username = username,
            Firstname = firstname,
            Surname = surname,
            MiddleName = middleName,
            PasswordHash = hashedPassword,
            Roles = new List<Role> { new() { Id = 1 } }
        };
        await _accountService.Create(account);
        return IAuthenticationService.RegistrationResult.Success;
    }

    public async Task<Account> Login(string username, string password)
    {
        var find = await _accountService.GetByUsername(username);
        if (find == null)
            throw new UserNotFoundException(username);
        var passwordResult = _passwordHasher.VerifyHashedPassword(find.PasswordHash, password);
        if (passwordResult != PasswordVerificationResult.Success)
            throw new InvalidPasswordException(username, password);
        return find;
    }
}