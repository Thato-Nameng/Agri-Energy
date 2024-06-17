using System;
using System.ComponentModel.DataAnnotations;

namespace Agri_Energy.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }

        [Required]
        public DateTime ProductionDate { get; set; } = DateTime.Now;

        [Required]
        public int FarmerId { get; set; }  // Keep as int
        public Farmer Farmer { get; set; }
    }
}
