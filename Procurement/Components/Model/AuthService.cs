using Firebase.Storage;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using FireBlazor.Components;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.JSInterop;
using Procurement.Components.Model;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;    
    private readonly string _apiKey ;    
    private readonly FirestoreDb _db ;
    private readonly FirebaseAuthProvider _authProvider;
    private readonly FirebaseAuth auth;
    private readonly IJSRuntime _js;
    public event Action<string, bool>? OnNotification; 
    public IConfiguration Configuration { get; set; }

    public virtual User CurrentUser { get; set; }
    public string CurrentUserToken { get; set; }

    public AuthService(HttpClient httpClient, IJSRuntime js, IConfiguration configuration)
    {
        Configuration = configuration;
        _httpClient = httpClient;
        _js = js;
        _apiKey = Configuration["Firebase:ApiKey"];
        _db= FirestoreDb.Create(Configuration["Firebase:DB"]);
    }

    private void Notify(string message, bool isError = false)
    {
        OnNotification?.Invoke(message, isError);
    }

    public string GetRoleFromToken(string idToken)
    {
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(idToken);

        
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
        try
        {
            if (CurrentUser.Email.ToLower() == "mohamedabbassit@gmail.com")
            {
                return "Admin";
            }
        }
        catch
        {
            Console.WriteLine("test");
        }
        

        return roleClaim?.Value ?? "User";
    }

    
    public async Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string role,string organization,bool approved,string fileN, string category, string firstname, string lastname)
    {
        try
        {
            var emailChecker = new EmailAddressAttribute();
            if (!emailChecker.IsValid(email))
            {                
                return (false, $"The email address provided is not in a valid format.");
            }
            var userArgs = new UserRecordArgs
            {
                Email = email,
                Password = password,
                EmailVerified = false,
            };

            var claims = new Dictionary<string, object>{
                { "role", role }
            };

            var result =await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
                        
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(result.Uid, claims);

            string? idToken = await GetIdTokenAsync(email, password);

            bool emailSent = await SendVerificationEmail(idToken);
                        

            DocumentReference docRef = _db.Collection("users").Document(result.Uid);
            await docRef.SetAsync(new
            {
                Email = email,
                Role = role,
                Approved = approved,
                category = category,
                Organization= organization,
                Firstname=firstname,
                LastName = lastname,
                CreatedAt = Timestamp.GetCurrentTimestamp(),
                fileName = fileN
            });

            if (!emailSent)
            {
                UserController uc = new UserController();
                await uc.DeleteUserAsync(result.Uid);
                return (false, "Please use a real working email");
            }

            Notify("Account created! Please check your inbox to verify your email before logging in.");
            return (true, null);
        }
        catch (FirebaseAuthException ex)
        {
            // Map common registration errors
            var message = ex.AuthErrorCode switch
            {
                AuthErrorCode.EmailAlreadyExists => "This email is already in use.",                
                _ => ex.Message
            };
            return (false, message);
        }
        catch (Exception ex)
        {
            return (false, $"An unexpected error occurred: {ex.Message}");
        }
    }

    
    public async Task<(bool Success, string? Token, string? Error, string role, bool approved)> SignInAsync(string email, string password)
    {
        var payload = new
        {
            email,
            password,
            returnSecureToken = true
        };       

        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}";

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<FirebaseSignInResponse>();                
                

                
                //DocumentReference docRef = db.Collection("Users").Document(email);
                Query query = _db.Collection("users").WhereEqualTo("Email", email).Limit(1);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                if (snapshot.Documents.Count > 0)
                {
                    DocumentSnapshot userDoc = snapshot.Documents[0];
                    userDoc.TryGetValue("Role", out string role);
                    userDoc.TryGetValue("Organization", out string organization);
                    userDoc.TryGetValue("Approved", out bool status);
                    if (status)
                    {
                        
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(data?.IdToken);
                        var isVerified = jwtToken.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value == "true";
                        
                        if (!isVerified)
                        {
                            return (false, null, "Please verify your email address before logging in.", "", false);
                        }
                        
                        CurrentUser = new User { Email = email, Password = password, Role = role, Organization = organization, Approved = status };
                        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", data?.IdToken);
                        await _js.InvokeVoidAsync("localStorage.setItem", "userEmail", email);
                        return (true, data?.IdToken, null, role, true);
                    }
                    else{
                        return (true, data?.IdToken, null, role, false);
                    }
                }                
            }

            var errorData = await response.Content.ReadFromJsonAsync<FirebaseErrorResponse>();
            return (false, null, errorData?.Error?.Message,"",false);
        }
        catch (Exception ex)
        {
            return (false, null, $"Connection error: {ex.Message}","", false);
        }
    }

    public async Task logOut()
    {
        if (CurrentUser != null)
        {
            CurrentUser = null;
            CurrentUserToken = null;
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _js.InvokeVoidAsync("localStorage.removeItem", "userEmail");
        }
    }

    public async Task InitializeAsync()
    {
        try
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            var email = await _js.InvokeAsync<string>("localStorage.getItem", "userEmail");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
            {
                CurrentUserToken = token;

                            
                Query query = _db.Collection("users").WhereEqualTo("Email", email).Limit(1);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                
                if (snapshot.Documents.Count > 0)
                {
                    var doc = snapshot.Documents[0];
                    doc.TryGetValue<string>("Organization", out string organization);
                    CurrentUser = new User
                    {
                        Email = email,
                        Role = doc.GetValue<string>("Role"),
                        Organization = organization,
                        Approved = doc.GetValue<bool>("Approved")
                    };
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
    }

    public async Task SaveAuthData(string token, string email)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        await _js.InvokeVoidAsync("localStorage.setItem", "userEmail", email);
    }

    public async Task<bool> IsEmailReal(string email)
    {
        using var client = new HttpClient();
        
        var response = await client.GetAsync($"https://emailvalidation.abstractapi.com/v1/?api_key=YOUR_KEY&email={email}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            return content.Contains("DELIVERABLE");
        }
        return false;
    }

    private async Task<bool> SendVerificationEmail(string token)
    {
        using var client = new HttpClient();

        
        var payload = new
        {
            requestType = "VERIFY_EMAIL",
            idToken = token           
        };

        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={_apiKey}";

        var response = await client.PostAsJsonAsync(url, payload);

        if (!response.IsSuccessStatusCode)
        {
            
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Firebase Error: {errorContent}");
        }

        return response.IsSuccessStatusCode;
    }

    private async Task<string?> GetIdTokenAsync(string email, string password)
    {
        using var client = new HttpClient();
        var payload = new { email, password, returnSecureToken = true };
               
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}";

        var response = await client.PostAsJsonAsync(url, payload);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            return doc.RootElement.GetProperty("idToken").GetString();
        }
        return null;
    }
    
}


public class FirebaseSignInResponse { public string IdToken { get; set; } = string.Empty; }
public class FirebaseErrorResponse { public FirebaseErrorDetails Error { get; set; } = new(); }
public class FirebaseErrorDetails { public string Message { get; set; } = string.Empty; }
