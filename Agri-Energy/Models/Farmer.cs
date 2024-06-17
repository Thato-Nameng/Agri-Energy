using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agri_Energy.Models
{
    public class Farmer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public bool MustChangePassword { get; set; } = true;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        public string? ApplicationUserId { get; set; }  // Make ApplicationUserId nullable
    }

}
