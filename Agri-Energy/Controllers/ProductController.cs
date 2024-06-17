using Agri_Energy.Data;
using Agri_Energy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agri_Energy.Controllers
{
    [Authorize(Roles = "Farmer")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<ProductController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.FarmerId.HasValue)
            {
                var products = _context.Products.Where(p => p.FarmerId == user.FarmerId.Value);
                return View(await products.ToListAsync());
            }
            return View(new List<Product>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid && user.FarmerId.HasValue)
            {
                product.FarmerId = user.FarmerId.Value;
                try
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while saving the product.");
                    ModelState.AddModelError("", "An error occurred while saving the product.");
                }
            }
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (product == null || !user.FarmerId.HasValue || product.FarmerId != user.FarmerId.Value)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (!user.FarmerId.HasValue || product.FarmerId != user.FarmerId.Value)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the product.");
                    ModelState.AddModelError("", "An error occurred while updating the product.");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (!user.FarmerId.HasValue || product.FarmerId != user.FarmerId.Value)
            {
                return Unauthorized();
            }
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product.");
                ModelState.AddModelError("", "An error occurred while deleting the product.");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public async Task<IActionResult> List(string searchString)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.FarmerId.HasValue)
            {
                var products = _context.Products.Where(p => p.FarmerId == user.FarmerId.Value);
                if (!string.IsNullOrEmpty(searchString))
                {
                    products = products.Where(p => p.ProductName.Contains(searchString));
                }
                return View(await products.ToListAsync());
            }
            return View(new List<Product>());
        }
    }
}
