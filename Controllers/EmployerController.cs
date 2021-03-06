using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Crewing.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Crewing.Controllers
{
    public class EmployerController : Controller
    {
        private CrewingContext context;

        public EmployerController(CrewingContext context)
        {
            this.context = context;
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

        public async Task<IActionResult> Preview()
        {
            var employers = context.Employers
                .Include(e => e.Reviews)
                .OrderByDescending(e => e.Rating);
            return View(await employers.ToListAsync());
        }

        public async Task<IActionResult> Details(string? companyName)
        {
            if (companyName == null || context.Employers == null)
            {
                return NotFound();
            }

            var employer = await context.Employers
                .Include(e => e.Vessels)
                .FirstOrDefaultAsync(e => e.Companyname == companyName);

            if (employer != null)
            {
                var reviews = await context.Reviews
                    .Where(r => r.Companyname == employer.Companyname)
                    .Include(r => r.Client)
                    .ToListAsync();
            }
            else
            {
                return NotFound();
            }

            return View("~/Views/Account/EmployerProfile.cshtml", employer);
        }

        // GET: Employer/CreateAgreement
        public async Task<IActionResult> CreateAgreement()
        {
            if (!User.IsInRole("Employer"))
                return NotFound();

            var vessels = await context.Vessels.Where(v => v.CompanynameNavigation!.Email == User.Identity!.Name).ToListAsync();
            ViewData["Vesselnumber"] = new SelectList(vessels, "Internationalnumber", "Internationalnumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAgreement([Bind("Vesselnumber")] Agreement agreement)
        {
            if (ModelState.IsValid)
            {
                context.Add(agreement);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Preview));
            }
            ViewBag.IsValid = ModelState.IsValid;

            var vessels = await context.Vessels.Where(v => v.CompanynameNavigation!.Email == User.Identity!.Name).ToListAsync();
            ViewData["Vesselnumber"] = new SelectList(vessels, "Internationalnumber", "Internationalnumber");

            return View(agreement);
        }

        //GET: Employer/CreateVessel
        public async Task<IActionResult> CreateVessel()
        {
            if (!User.IsInRole("Employer"))
                return NotFound();

            var vesseltypes = await context.Vesseltypes.ToListAsync();
            ViewData["Vesseltypeid"] = new SelectList(vesseltypes, "Id", "Name");

            Employer? employer = await context.Employers.FirstOrDefaultAsync(e => e.Email == User.Identity!.Name);
            if (employer == null)
                return NotFound();

            ViewBag.Companyname = employer.Companyname;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVessel([Bind("Internationalnumber,Vesselname,Vesseltypeid,Companyname,Workersamount,Location,Status")] Vessel vessel)
        {
            var company = await context.Employers.FindAsync(vessel.Companyname);
            var vesseltype = await context.Vesseltypes.FindAsync(vessel.Vesseltypeid);
            if (company != null)
                vessel.CompanynameNavigation = company;
            if (vesseltype != null)
                vessel.Vesseltype = vesseltype;

            if (ModelState.IsValid)
            {
                context.Add(vessel);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Preview));
            }
            ViewBag.IsValid = ModelState.IsValid;
            var vesseltypes = await context.Vesseltypes.ToListAsync();
            ViewData["Vesseltypeid"] = new SelectList(vesseltypes, "Id", "Name");

            Employer? employer = await context.Employers.FirstOrDefaultAsync(e => e.Email == User.Identity!.Name);
            if (employer == null)
                return NotFound();

            ViewBag.Companyname = employer.Companyname;
            return View(vessel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(ReviewCreationModel reviewForm)
        {
            Client? client = (Client?)await context.FindUserOrNullAsync(User.Identity!.Name!);
            if(client != null)
            {
                Review review = new Review()
                {
                    Clientid = client.Id,
                    Companyname = reviewForm.Companyname,
                    Comment = reviewForm.Comment,
                    Estimation = reviewForm.Estimation,
                    Datetime = DateTime.Now
                };
                if (ModelState.IsValid)
                {
                    context.Add(review);
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Preview));
                }
            }
            ViewBag.IsValid = ModelState.IsValid;
            return View("~/Views/Employer/ReviewForm.cshtml", reviewForm);
        }
    }
}
