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

namespace Crewing.Controllers
{
    public class AccountController : Controller
    {
        private readonly CrewingContext context;
        private IConfiguration configuration;

        public AccountController(CrewingContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
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
                    User? user = await FindUserOrNull(model.Email!);
                    if (user != null)
                        throw new Exception("User with such email exists");

                    user = await FindUserOrNull(model.PhoneNumber!);
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
                    User? user = await FindCompanyByNameOrNull(model.CompanyName!);
                    if (user != null)
                        throw new Exception("Company with such name exists");

                    user = await FindUserOrNull(model.Email!);
                    if (user != null)
                        throw new Exception("User with such email exists");

                    user = await FindUserOrNull(model.PhoneNumber!);
                    if (user != null)
                        throw new Exception("User with such phone exists");

                    user = new Employer
                    {
                        Companyname = model.CompanyName!,
                        Email = model.Email!,
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
                    User? user = await FindUserOrNull(model.EmailOrPhone!);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "User with such email or phone doesn't exist.");
                        return View(model);
                    }
                    await context.Database.CloseConnectionAsync();
                    context.Database.SetConnectionString($"Server=localhost; Port=5432; Database=Crewing; Username={user.GetType().Name.ToLower()}{user.Phonenumber.Remove(0, 1)}; Password={model.Password}");
                    try
                    {
                        await context.Database.OpenConnectionAsync();
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Incorrect password.");
                        return View(model);
                    }
                    await context.SaveChangesAsync();
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
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
            await context.Database.OpenConnectionAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task<User?> FindUserOrNull(string emailOrPhone)
        {
            User? user = await context.Clients.FirstOrDefaultAsync(c => c.Email == emailOrPhone || c.Phonenumber == emailOrPhone);
            if (user != null)
                return user;
            user = await context.Employers.FirstOrDefaultAsync(er => er.Email == emailOrPhone || er.Phonenumber == emailOrPhone);
            if (user != null)
                return user;
            user = await context.Employees.FirstOrDefaultAsync(e => e.Email == emailOrPhone || e.Phonenumber == emailOrPhone);
            if (user != null)
                return user;
            return null;
        }
        
        private async Task<Employer?> FindCompanyByNameOrNull(string companyName)
        {
            Employer? employer = await context.Employers.FirstOrDefaultAsync(er => er.Companyname == companyName);
            return employer;
        }
    }
}
