using Agri_Energy.Data;
using Agri_Energy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Agri_Energy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CreateFarmer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFarmer(Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = farmer.Email,
                    Email = farmer.Email,
                    FirstName = farmer.Name,
                    LastName = farmer.Surname
                };

                var defaultPassword = "DefaultPassword123!";
                var result = await _userManager.CreateAsync(user, defaultPassword);

                if (result.Succeeded)
                {
                    var createdUser = await _userManager.FindByEmailAsync(farmer.Email);

                    if (createdUser != null)
                    {
                        farmer.ApplicationUserId = createdUser.Id; // Set the ApplicationUser Id
                        farmer.MustChangePassword = true;
                        _context.Farmers.Add(farmer);

                        try
                        {
                            await _context.SaveChangesAsync();
                            await _userManager.AddToRoleAsync(createdUser, "Farmer");
                            TempData["SuccessMessage"] = "New farmer has been created successfully.";
                            return RedirectToAction("ListFarmers");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An error occurred while saving the farmer to the database.");
                            ModelState.AddModelError(string.Empty, "An error occurred while saving the farmer.");
                        }
                    }
                    else
                    {
                        _logger.LogError("Created user not found. Email: {Email}", farmer.Email);
                        ModelState.AddModelError(string.Empty, "Error creating user. Please try again.");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Error creating user: {Error}", error.Description);
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("Model state error: {Error}", modelError.ErrorMessage);
                }
            }

            return View(farmer);
        }

        [HttpGet]
        public IActionResult ListFarmers(string searchString)
        {
            var farmers = from f in _context.Farmers
                          select f;

            if (!string.IsNullOrEmpty(searchString))
            {
                farmers = farmers.Where(s => s.Name.Contains(searchString) || s.Surname.Contains(searchString));
            }

            return View(farmers.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> ViewFarmerProducts(int id, string searchString)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null)
            {
                return NotFound();
            }

            var products = _context.Products.Where(p => p.FarmerId == id);
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.ProductName.Contains(searchString));
            }

            var viewModel = new FarmerProductsViewModel
            {
                Farmer = farmer,
                Products = await products.ToListAsync()
            };

            return View(viewModel);
        }
    }

    public class FarmerProductsViewModel
    {
        public Farmer Farmer { get; set; }
        public List<Product> Products { get; set; }
    }
}
