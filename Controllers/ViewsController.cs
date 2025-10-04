//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using I_Attend.Data;
//using I_Attend.Models;
//using Microsoft.Extensions.Logging;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication;

//namespace I_Attend.Controllers
//{
//    public class ViewsController : Controller
//    {
//        private readonly I_AttendDAO _context;
//        private readonly ILogger<ViewsController> _logger;

//        public ViewsController(I_AttendDAO context, ILogger<ViewsController> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var views = await _context.GetViewsAsync();
//            return View(views);
//        }

//        [Authorize(Policy = "AuthenticatedOnly")]
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }
//            var view = (await _context.GetViewsAsync()).FirstOrDefault(m => m.Id == id);
//            if (view == null)
//            {
//                return NotFound();
//            }
//            return View(view);
//        }

//        public IActionResult Create()
//        {
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(View model)
//        {
//            if ((await _context.GetViewsAsync()).Any(v => v.Matric_Number == model.Matric_Number))
//            {
//                ModelState.AddModelError("Matric_Number", "Matric number is already taken.");
//                return View(model);
//            }
//            if (ModelState.IsValid)
//            {
//                await _context.AddViewAsync(model);
//                // Sign in the user after registration
//                var claims = new List<Claim>
//                {
//                    new Claim(ClaimTypes.Name, model.UserNames),
//                    new Claim(ClaimTypes.Email, model.Email),
//                    new Claim("MatricNumber", model.Matric_Number)
//                };
//                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
//                return RedirectToAction("Capture", "Camera");
//            }
//            return View(model);
//        }

//        [Authorize(Policy = "AuthenticatedOnly")]
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }
//            var view = (await _context.GetViewsAsync()).FirstOrDefault(m => m.Id == id);
//            if (view == null)
//            {
//                return NotFound();
//            }
//            return View(view);
//        }

//        [Authorize(Policy = "AuthenticatedOnly")]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, View view)
//        {
//            if (id != view.Id)
//            {
//                return NotFound();
//            }
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    await _context.UpdateViewAsync(view);
//                    return RedirectToAction(nameof(Index));
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error updating view with ID {Id}", view.Id);
//                    if (!await _context.ViewExistsAsync(view.Id))
//                    {
//                        return NotFound();
//                    }
//                    throw;
//                }
//            }
//            return View(view);
//        }

//        [Authorize(Policy = "AuthenticatedOnly")]
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }
//            var view = (await _context.GetViewsAsync()).FirstOrDefault(m => m.Id == id);
//            if (view == null)
//            {
//                return NotFound();
//            }
//            return View(view);
//        }

//        [Authorize(Policy = "AuthenticatedOnly")]
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            await _context.DeleteViewAsync(id);
//            return RedirectToAction(nameof(Index));
//        }

//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Login(LoginViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await _context.GetUserCredentialsAsync(model.Email, model.Password, model.Matric_Number);
//                if (user != null)
//                {
//                    var claims = new List<Claim>
//                    {
//                        new Claim(ClaimTypes.Name, user.UserNames),
//                        new Claim(ClaimTypes.Email, user.Email),
//                        new Claim("MatricNumber", user.Matric_Number)
//                    };

//                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//                    var authProperties = new AuthenticationProperties { };

//                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
//                    HttpContext.Session.SetString("UserId", user.Email.ToString());
//                    return RedirectToAction("Index", "Home");
//                }
//                ModelState.AddModelError("", "Invalid matric number, password, or email.");
//            }
//            return View(model);
//        }

//        public IActionResult Register()
//        {
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Register(RegisterViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                if ((await _context.GetViewsAsync()).Any(v => v.Matric_Number == model.Matric_Number))
//                {
//                    ModelState.AddModelError("Matric_Number", "Matric number is already taken.");
//                    return View(model);
//                }
//                var view = new View
//                {
//                    UserNames = model.UserNames,
//                    Department = model.Department,
//                    Email = model.Email,
//                    Matric_Number = model.Matric_Number,
//                    Password = model.Password,
//                    CourseCode = model.CourseCode
//                };
//                if (await _context.RegisterCredentialsAsync(view))
//                {
//                    // Sign in the user after registration
//                    var claims = new List<Claim>
//                    {
//                        new Claim(ClaimTypes.Name, view.UserNames),
//                        new Claim(ClaimTypes.Email, view.Email),
//                        new Claim("MatricNumber", view.Matric_Number)
//                    };
//                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
//                    return RedirectToAction("Capture", "Camera");
//                }
//                ModelState.AddModelError("", "Registration failed. Please try again.");
//            }
//            return View(model);
//        }

//        public async Task<IActionResult> Logout()
//        {
//            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//            HttpContext.Session.Clear();
//            return RedirectToAction("Login");
//        }
//    }
//}










using I_Attend.Data;
using I_Attend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "User")]
        public IActionResult CreateCourse()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Matric_Number, Course_code")] Course_List model)
        {
            if (ModelState.IsValid)
            {
                var user = (await _context.GetViewsAsync()).Any(v => v.Matric_Number == model.Matric_Number);
                if (user == null)
                {
                    ModelState.AddModelError("Matric_Number", "Matric number does not exists.");
                    return View(model);
                }
                // Create a new View object with all required properties
                var view = new View
                {
                    //UserNames = model.UserNames,
                    //Department = model.Department,
                    //Email = model.Email,
                    Matric_Number = model.Matric_Number,
                    //Password = model.Password,
                    Course_code = model.Course_code
                    //ImageData = model.ImageData // Ensure this is provided or make it optional
                };


                // Add the view to the database
                //await _context.AddViewAsync(view);
                //return RedirectToAction(nameof(Index));


                // Optionally sign in the user after creation
                //var claims = new List<Claim>
                //{
                //    //new Claim(ClaimTypes.Name, view.UserNames),
                //    //new Claim(ClaimTypes.Email, view.Email),
                //    new Claim("Course_code", view.Course_code)
                //};
                //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                //return RedirectToAction("Capture", "Camera");

                if (ModelState.IsValid)
                {
                    await _context.AddViewAsync(view);
                    return RedirectToAction(nameof(CreateCourse));
                }
                ModelState.AddModelError("", "Registration failed. Please try again.");
            }

            return View(model);
        }


        //if ((await _context.GetViewsAsync()).Any(v => v.Matric_Number == model.Matric_Number))
        //{
        //    ModelState.AddModelError("Matric_Number", "Matric number is already taken.");
        //    return View(model);
        //}
        //var view = new View
        //{
        //    Course_code = model.Course_code
        //};
        //if (ModelState.IsValid)
        //{
        //    await _context.UpdateViewAsync(model);
        //    return RedirectToAction(nameof(Index));
        //}
        //return View(model);
        //View model = new View();
        //model.CourseList.Add(new CourseListItem{Text= "Computer Graphics Animation",Value= "ECE5250"}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var view1 = (await _context.GetViewsAsync()).FirstOrDefault(m => m.Id == id);
            if (view1 == null)
            {
                return NotFound();
            }
            var view = new View
            {
                UserNames = view1.UserNames,
                Department = view1.Department,
                Email = view1.Email,
                Matric_Number = view1.Matric_Number,
                Password = view1.Password,
                Course_code = view1.Course_code
            };

            //var view = (await _context.GetViewsAsync()).FirstOrDefault(m => m.Id == id);
            //if (view == null)
            //{
            //    return NotFound();
            //}
            return View(view);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(View view)
        {
            //if (id != view.Id)
            //{
            //    return NotFound();
            //}
            if (ModelState.IsValid)
            {
                return View(view);
            }
            

            var view1 = (await _context.GetViewsAsync()).FirstOrDefault(m => m.Id == view.Id);
            if (view1 == null)
            {
                return NotFound();
            }

            if ((await _context.GetViewsAsync()).Any(v => v.Matric_Number == view.Matric_Number))
            {
                ModelState.AddModelError("Matric_Number", "Matric number is already taken.");
                return View(view);
            }

            // Update values
            view1.UserNames = view.UserNames;
            view1.Department = view.Department;
            view1.Email = view.Email;
            view1.Matric_Number = view.Matric_Number;
            view1.Password = view.Password;
            view1.Course_code = view.Course_code;

             

            //var views = _context.UpdateView();
            //await _context.UpdateViewAsync(view);
            //var existingViews = views.FirstOrDefault(v => v.Id == id);

            await _context.SaveChangesAsync(view1);

            //_logger.LogError("", "Error updating view with ID {Id}", view1.Id);
            ModelState.AddModelError("", "An error occurred while saving. Please try again.");

            //if (!await _context.ViewExistsAsync(id))
            //{
            //    return NotFound();
            //}


            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _context.DeleteViewAsync(id);
            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ViewExists(int id)
        {
            var views = (await _context.GetViewsAsync()).FirstOrDefault(e => e.Id == id);
            return View(views);   //Any(e => e.Id == id);
        }

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
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserNames),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("MatricNumber", user.Matric_Number),
                        new Claim(ClaimTypes.Role, "User")
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {

                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    // Implement session or cookie-based authentication here
                    HttpContext.Session.SetString("UserId", user.Email.ToString());
                    return RedirectToAction("User", "Home");
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
                if ((await _context.GetViewsAsync()).Any(v => v.Matric_Number == model.Matric_Number) || (await _context.GetViewsAsync()).Any(v => v.Email == model.Email))
                {
                    ModelState.AddModelError("Matric_Number", "Matric number is already taken.");
                    ModelState.AddModelError("Email", "Email can't be used, it's already used.");
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
                    return RedirectToAction("Capture", "Camera");
                }
                ModelState.AddModelError("", "Registration failed. Please try again.");
            }
            return View(model);
            //return View("~/Views/Camera/Profile.cshtml", view);
        }

        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.User == "Admin1" && model.Password == "123Pa$$word")
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, "Admin1"),
                        new Claim("Password", model.Password),
                        new Claim(ClaimTypes.Role, "Admin")

                    };

                    var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {

                    };

                    await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);
                    // Implement session or cookie-based authentication here
                    HttpContext.Session.SetString("UserId", model.User.ToString());
                    return RedirectToAction("Admin", "Home");

                }
                ModelState.AddModelError("", "Invalid User or password.");

            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");

        }

        ////public IActionResult Logout()
        ////{
        ////    HttpContext.Session.Clear();
        ////    return RedirectToAction("Login");
        //}
    }
}