using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Models;

namespace WoodenWorkshop.Crm.Api.Services.Abstractions;

public interface IAuthorizationService
{
    public Task<AuthorizationResult> AuthorizeAsync(UserCredentialsDto userCredentials, string ipAddress);

    public Task<AuthorizationResult> RefreshSessionAsync(string refreshToken);
}