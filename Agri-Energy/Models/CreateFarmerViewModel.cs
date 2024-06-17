using System.ComponentModel.DataAnnotations;
using System;

namespace Agri_Energy.Models
{
    public class CreateFarmerViewModel
    {
        [Required]
        public string FirstName { get; set; }

        
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}