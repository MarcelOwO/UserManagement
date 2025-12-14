using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using UserManagement.Core.Dto.Auth;
using UserManagement.Web.Models;

namespace UserManagement.Web.Services;

public class CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
    : AuthenticationStateProvider
{
    private const string JwtTokenKey = "jwtToken";

    private readonly AuthenticationState _anonymousState =
        new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        var loginDto = new LoginDto(email, password);

        var response = await httpClient.PostAsJsonAsync("api/login", loginDto);

        if (!response.IsSuccessStatusCode)
        {
            return new LoginResult(false, "Login failed.");
        }

        var authResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        if (string.IsNullOrWhiteSpace(authResponse?.Token))
        {
            return new LoginResult(false, "Login failed: token missing.");
        }

        await localStorage.SetItemAsStringAsync(JwtTokenKey, authResponse.Token);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return new LoginResult(true, null);
    }

    public async Task LogoutAsync()
    {
        await httpClient.PostAsync("api/logout", null);

        await localStorage.RemoveItemAsync(JwtTokenKey);

        NotifyAuthenticationStateChanged(Task.FromResult(_anonymousState));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await localStorage.GetItemAsStringAsync(JwtTokenKey);
        if (string.IsNullOrWhiteSpace(savedToken))
        {
            return _anonymousState;
        }

        try
        {
            var claims = ParseClaimsfromJwt(savedToken.Replace(@"\", ""));
            if (claims == null || !IsTokenValid(claims))
            {
                await localStorage.RemoveItemAsync(JwtTokenKey);
                return _anonymousState;
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var principal = new ClaimsPrincipal(identity);
            return new AuthenticationState(principal);
        }
        catch
        {
            await localStorage.RemoveItemAsync(JwtTokenKey);
            return _anonymousState;
        }
    }

    private static IEnumerable<Claim> ParseClaimsfromJwt(string jwt)
    {
        var claims = new List<Claim>();

        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            claims.AddRange((keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty))));
        }

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }

    private static bool IsTokenValid(IEnumerable<Claim> claims)
    {
        var expirationClaim = claims.FirstOrDefault(c => c.Type == "exp");
        
        if (expirationClaim == null)
        {
            return true;
        }

        if (long.TryParse(expirationClaim.Value, out var expTime))
        {
            var expiration = DateTimeOffset.FromUnixTimeSeconds(expTime);
            return expiration > DateTimeOffset.UtcNow.AddMinutes(5);
        }

        return false;
    }
}