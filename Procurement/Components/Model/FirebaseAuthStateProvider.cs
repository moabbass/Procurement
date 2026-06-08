using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Procurement.Components.Model;

public class FirebaseAuthStateProvider : AuthenticationStateProvider
{
    private AuthenticationState _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

    public void NotifyUserAuthentication(User session)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, session.Email),
            new Claim(ClaimTypes.Role, session.Role)
        };

        var identity = new ClaimsIdentity(claims, "Firebase");
        var user = new ClaimsPrincipal(identity);
        var state = Task.FromResult(new AuthenticationState(user));

        NotifyAuthenticationStateChanged(state);
    }

    public void NotifyUserLogout()
    {
        var state = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(state);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Default to anonymous; logic to check LocalStorage can go here later
        return Task.FromResult(_anonymous);
    }
}