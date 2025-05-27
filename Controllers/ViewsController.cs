using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using I_Attend.Data;
using I_Attend.Models;
using Microsoft.Extensions.Logging;

namespace I_Attend.Controllers
{
    public class ViewsController : Controller
    {
        private readonly I_AttendDAO _context;
        private readonly ILogger<ViewsController> _logger;

        public ViewsController(I_AttendDAO context, ILogger<ViewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var views = await _context.GetViewsAsync();
            return View(views);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var view = (await _context.GetViewsAsync()).FirstOrDefault(m => m.Id == id);
            if (view == null)
            {
                return NotFound();
            }
            return View(view);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(View model)
        {
            if ((await _context.GetViewsAsync()).Any(v => v.Matric_Number == model.Matric_Number))
            {
                ModelState.AddModelError("Matric_Number", "Matric number is already taken.");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                await _context.AddViewAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var view = (await _context.GetViewsAsync()).FirstOrDefault(m => m.Id == id);
            if (view == null)
            {
                return NotFound();
            }
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, View view)
        {
            if (id != view.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.UpdateViewAsync(view);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating view with ID {Id}", view.Id);
                    if (!await _context.ViewExistsAsync(view.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(view);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var view = (await _context.GetViewsAsync()).FirstOrDefault(m => m.Id == id);
            if (view == null)
            {
                return NotFound();
            }
            return View(view);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _context.DeleteViewAsync(id);
            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> ViewExists(int id)
        //{
        //   var views = (await _context.GetViewsAsync()).FirstOrDefault(e=> e.Id == id);
        //    return View(views);   //Any(e => e.Id == id);
        //}

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.GetUserCredentialsAsync(model.Email, model.Password, model.Matric_Number);
                if (user != null)
                {
                    // Implement session or cookie-based authentication here
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid matric number, password, or email.");
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if ((await _context.GetViewsAsync()).Any(v => v.Matric_Number == model.Matric_Number))
                {
                    ModelState.AddModelError("Matric_Number", "Matric number is already taken.");
                    return View(model);
                }
                var view = new View
                {
                    UserNames = model.UserNames,
                    Department = model.Department,
                    Email = model.Email,
                    Matric_Number = model.Matric_Number,
                    Password = model.Password
                };
                if (await _context.RegisterCredentialsAsync(view))
                {
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Registration failed. Please try again.");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}