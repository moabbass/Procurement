using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;

namespace Procurement.Components.Model
{
    public class UserController : IUserController
    {
        private readonly FirestoreDb _db = FirestoreDb.Create("procurement-ed5cc");
        public UserController() { }

        public UserController(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<List<User>> GetUsersByCategory(string category)
        {
            
            Query query = _db.Collection("users").WhereEqualTo("category", category);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(d => new User
            {
                Email = d.GetValue<string>("Email"),
                Organization = d.GetValue<string>("Organization"),
                Role = d.GetValue<string>("Role"),
                Approved = d.GetValue<bool>("Approved"),
                fileName = d.GetValue<string>("fileName"),
                category = d.GetValue<string>("category")
            }).ToList();
/*
            
*/
        }

        public virtual async Task<List<User>> GetUsersByRole(string role)
        {
            
            Query query = _db.Collection("users").WhereEqualTo("Role", role);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(d => new User
            {
                Email = d.TryGetValue("Email", out string email) ? email : string.Empty,
                Organization = d.TryGetValue("Organization", out string org) ? org : "N/A",
                Role = d.TryGetValue("Role", out string r) ? r : role,
                Approved = d.TryGetValue("Approved", out bool app) ? app : false,
                fileName = d.TryGetValue("fileName", out string fName) ? fName : string.Empty,
                category = d.TryGetValue("category", out string cat) ? cat : "General",
                FirstName = d.TryGetValue("Firstname", out string FirstName) ? FirstName : string.Empty,
                LastName = d.TryGetValue("LastName", out string LastName) ? LastName : string.Empty,

                
                ID = d.Id
            }).ToList();
                      
        }
        public virtual async Task<User> GetUsersByEmail(string email)
        {
           

            Query query = _db.Collection("users").WhereEqualTo("Email", email).Limit(1);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            // Check if any document exists
            var doc = snapshot.Documents.FirstOrDefault();

            if (doc == null)
            {
                return null;
            }

            return new User
            {
                Email = doc.TryGetValue("Email", out string e) ? e : string.Empty,
                Organization = doc.TryGetValue("Organization", out string org) ? org : "N/A",
                Role = doc.TryGetValue("Role", out string role) ? role : string.Empty,
                Approved = doc.TryGetValue("Approved", out bool app) ? app : false,
                fileName = doc.TryGetValue("fileName", out string fName) ? fName : string.Empty,
                category = doc.TryGetValue("category", out string cat) ? cat : "General",
                FirstName = doc.TryGetValue("Firstname", out string fNameValue) ? fNameValue : string.Empty,
                LastName = doc.TryGetValue("LastName", out string lNameValue) ? lNameValue : string.Empty,
                ID = doc.Id
            };

        }


        public virtual async Task<List<User>> GetUserByOrganization(string organization)
        {
            Query query = _db.Collection("users").WhereEqualTo("Organization", organization);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(doc => {
                User user = doc.ConvertTo<User>();
                return user;

            }).ToList();
        }


        public virtual async Task DeleteUserAsync(string userID)
        {
            try
            {
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(userID);
                await _db.Collection("users").Document(userID).DeleteAsync();
            }
            catch (Exception ex)
            {

            }
                       
        }
        
        public virtual async Task UpdateUserAsync(User user)
        {
            var userUpdate = new Dictionary<string, object>{
                { "Email", user.Email },
                { "Firstname", user.FirstName },
                { "LastName", user.LastName },
                { "Organization", user.Organization },
                { "Role", user.Role },
                { "Approved", user.Approved },
                { "category", user.category ?? "" }, 
                { "fileName", user.fileName ?? "" }
            };
            await _db.Collection("users").Document(user.ID).SetAsync(userUpdate, SetOptions.Overwrite);

            var args = new UserRecordArgs
            {
                Uid = user.ID,
                Email = user.Email,
                DisplayName = $"{user.FirstName} {user.LastName}"
                // Note: You can also update Password here if needed: Password = newPassword
            };
            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);

        }

    }
}
