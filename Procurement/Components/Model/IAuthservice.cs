using Procurement.Components.Model;

namespace Procurement.Components.Model
{
    public interface IAuthService
    {
        // State Properties
        User CurrentUser { get; set; }
        string CurrentUserToken { get; set; }

        // Events
        event Action<string, bool>? OnNotification;

        /// <summary>
        /// Registers a new user in Firebase Auth, sets custom claims (roles), 
        /// and creates a corresponding document in Firestore.
        /// </summary>
        Task<(bool Success, string? Error)> RegisterAsync(
            string email,
            string password,
            string role,
            string organization,
            bool approved,
            string category,
            string fileName,
            string firstName,
            string lastName);

        /// <summary>
        /// Authenticates a user with email and password, fetches their 
        /// Firestore profile, and persists the session.
        /// </summary>
        Task<(bool Success, string? Token, string? Error, string role, bool approved)> SignInAsync(
            string email,
            string password);

        /// <summary>
        /// Clears the current user state and removes tokens from local storage.
        /// </summary>
        Task logOut();

        /// <summary>
        /// Attempts to restore the user session from local storage on app startup.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Manually persists authentication tokens to local storage.
        /// </summary>
        Task SaveAuthData(string token, string email);

        /// <summary>
        /// Decodes the JWT token to retrieve the user's role.
        /// </summary>
        string GetRoleFromToken(string idToken);

        /// <summary>
        /// Performs an external check to verify if the email address is deliverable.
        /// </summary>
        Task<bool> IsEmailReal(string email);
    }
}