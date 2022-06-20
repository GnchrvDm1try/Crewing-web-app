using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Crewing.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Crewing.Controllers
{
    public class AccountController : Controller
    {
        private CrewingContext context;
        private IConfiguration configuration;

        public AccountController(CrewingContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (User.Identity!.IsAuthenticated)
            {
                string password = User.Claims.FirstOrDefault(c => c.Type == "password")!.Value;
                string role = User.Claims.FirstOrDefault(c => c.Type.Contains("role"))!.Value;
                User? user = await this.context.FindUserOrNullAsync(User.Identity.Name!);
                if (user == null)
                    throw new Exception("Unable to find the current user in the database");
                DbContextOptionsBuilder<CrewingContext> optionsBuilder = new DbContextOptionsBuilder<CrewingContext>();
                var options = optionsBuilder
                    .UseNpgsql($"Server=localhost; Port=5432; Database=Crewing; Username={role.ToLower()}{user.Phonenumber.Remove(0, 1)}; Password={password}")
                    .Options;
                using (CrewingContext crewingContext = new CrewingContext(options))
                {
                    await this.context.DisposeAsync();
                    this.context = crewingContext;
                    await next();
                }
            }
            else
            {
                await next();
            }
        }

        public async Task<IActionResult> Profile()
        {
            if (User.IsInRole("Client"))
            {
                Client? client = await context.Clients.FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
                if(client != null)
                {
                    var contracts = await context.Contracts
                        .Where(c => c.Clientid == client.Id)
                        .Include(c => c.Vacancy.Sailorpost)
                        .Include(c => c.Vacancy.AgreementnumberNavigation.VesselnumberNavigation)
                        .ToListAsync();
                    return View("ClientProfile", client);
                }
            }
            else if (User.IsInRole("Employer"))
            {
                Employer? employer = await context.Employers
                    .Include(e => e.Vessels)
                    .FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
                if(employer != null)
                {
                    var reviews = await context.Reviews
                        .Where(r => r.Companyname == employer.Companyname)
                        .Include(r => r.Client)
                        .ToListAsync();
                    return View("EmployerProfile", employer);
                }
            }
            return NotFound();
        }

        public IActionResult ClientRegister()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientRegister(ClientRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User? user = await context.FindUserOrNullAsync(model.Email!);
                    if (user != null)
                        throw new Exception("User with such email exists");

                    user = await context.FindUserOrNullAsync(model.PhoneNumber!);
                    if (user != null)
                        throw new Exception("User with such phone exists");

                    user = new Client
                    {
                        Firstname = model.FirstName!,
                        Lastname = model.LastName!,
                        Email = model.Email!,
                        Phonenumber = model.PhoneNumber!,
                        Registrationdate = DateOnly.FromDateTime(DateTime.Now),
                        Status = "Looking for a job",
                        Birthdate = DateOnly.FromDateTime(model.BirthDate),
                        Ismale = model.IsMale
                    };

                    await context.Database.ExecuteSqlRawAsync($"CREATE USER {typeof(Client).Name}{model.PhoneNumber!.Remove(0, 1)} " +
                        $"WITH LOGIN PASSWORD '{model.Password}' IN ROLE {typeof(Client).Name.ToLower()}");
                    await context.Clients.AddAsync((Client)user);
                    await context.SaveChangesAsync();

                    await Login(new UserLoginModel
                    {
                        EmailOrPhone = model.Email,
                        Password = model.Password
                    });

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            ViewBag.IsValid = ModelState.IsValid;
            return View("ClientRegister", model);
        }

        public IActionResult EmployerRegister()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployerRegister(EmployerRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User? user = await context.FindCompanyByNameOrNullAsync(model.CompanyName!);
                    if (user != null)
                        throw new Exception("Company with such name exists");

                    user = await context.FindUserOrNullAsync(model.Email!);
                    if (user != null)
                        throw new Exception("User with such email exists");

                    user = await context.FindUserOrNullAsync(model.PhoneNumber!);
                    if (user != null)
                        throw new Exception("User with such phone exists");

                    user = new Employer
                    {
                        Companyname = model.CompanyName!,
                        Email = model.Email!,
                        Registrationdate = DateOnly.FromDateTime(DateTime.Now),
                        Phonenumber = model.PhoneNumber!,
                        Rating = 0.0
                    };

                    await context.Database.ExecuteSqlRawAsync($"CREATE USER {typeof(Employer).Name}{model.PhoneNumber!.Remove(0, 1)} " +
                        $"WITH LOGIN PASSWORD '{model.Password}' IN ROLE {typeof(Employer).Name.ToLower()}");
                    await context.Employers.AddAsync((Employer)user);
                    await context.SaveChangesAsync();

                    await Login(new UserLoginModel
                    {
                        EmailOrPhone = model.Email,
                        Password = model.Password
                    });

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            ViewBag.IsValid = ModelState.IsValid;
            return View("EmployerRegister", model);
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            ViewBag.IsValid = false;
            if(ModelState.IsValid)
            {
                try
                {
                    User? user = await context.FindUserOrNullAsync(model.EmailOrPhone!);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "User with such email or phone doesn't exist.");
                        return View(model);
                    }

                    string role;
                    if (user is Employee)
                    {
                        var employeePosts = await context.EmployeePosts
                            .Where(ep => ep.Employeeid == ((Employee)user).Id)
                            .ToListAsync();
                        var posts = await context.Posts.ToListAsync();
                        role = ((Employee)user).EmployeePosts
                            .OrderByDescending(e => e.Hiringdate)
                            .First().Post.Name;
                    }
                    else
                        role = user.GetType().Name;

                    await context.Database.CloseConnectionAsync();
                    context.Database.SetConnectionString($"Server=localhost; Port=5432; Database=Crewing; Username={role.ToLower()}{user.Phonenumber.Remove(0, 1)}; Password={model.Password}");
                    try
                    {
                        await context.Database.OpenConnectionAsync();
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Incorrect password.");
                        return View(model);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
                        new Claim("password", model.Password!)
                    };
                    ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                    return RedirectToAction("Index", "Home");
                }
                catch
                {
                    ModelState.AddModelError("", "Unknown error.");
                }
            }
            else
                ModelState.AddModelError("", "Data has not been validated.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await context.Database.CloseConnectionAsync();
            context.Database.SetConnectionString(configuration.GetConnectionString("GuestConnection"));
            return RedirectToAction("Index", "Home");
        }
    }
}
