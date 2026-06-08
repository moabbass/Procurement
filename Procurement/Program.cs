//using FireBlazor;
using FirebaseAdmin;
using FireBlazor;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Procurement.Components;
using Procurement.Components.Model;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/login";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
        options.AccessDeniedPath = "/access-denied";
    });

var cultureInfo = new CultureInfo("en-GB");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Initialize Firebase Admin
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "procurement-ed5cc-firebase-adminsdk-fbsvc-e6ecc718b5.json");
if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile("procurement-ed5cc-firebase-adminsdk-fbsvc-e6ecc718b5.json")
    });
}
  

//Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "procurement-ed5cc-firebase-adminsdk-fbsvc-e6ecc718b5.json");

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<FirebaseAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<FirebaseAuthStateProvider>());

builder.Services.AddScoped<BidTracker>();

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpClient();
builder.Services.AddScoped<PDFValidatorService>();

builder.Services.AddFirebase(options => options
    .WithProject("procurement-ed5cc")
    .WithApiKey("AIzaSyCkRMLxMREXtSnzAzax-KP1LXjQh8hUcvo")
    .WithAuthDomain("procurement-ed5cc.firebaseapp.com")
    .UseAuth(auth => auth.EnableEmailPassword())
);


builder.Services.AddScoped<FileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
