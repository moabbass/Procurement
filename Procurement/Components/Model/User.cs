using System.ComponentModel.DataAnnotations;

namespace Procurement.Components.Model
{
    public class User
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;        
        public string Organization {  get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        public string FirstName {  get; set; }
        public string LastName {  get; set; }
        public string Role { get; set; } 
        public bool Approved { get; set; }
        public string pdfUrl {  get; set; }
        public string fileName {  get; set; }
        public string category {  get; set; }

        public string ID {  get; set; }
        public User()
        {
            
        }
    }

    
}
