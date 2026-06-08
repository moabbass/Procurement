namespace Procurement.Components.Model
{
    public interface IUserController
    {
        Task<List<User>> GetUsersByCategory(string category);
        Task<List<User>> GetUsersByRole(string role);
        Task<User> GetUsersByEmail(string email);
        Task<List<User>> GetUserByOrganization(string organization);
        Task DeleteUserAsync(string userID);
        Task UpdateUserAsync(User user);
    }
}
