using Microsoft.AspNetCore.Identity;

namespace Agri_Energy.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? FarmerId { get; set; } // Nullable to handle non-farmer users
    }
}
