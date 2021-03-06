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
    public class VacancyController : Controller
    {
        private CrewingContext context;

        public VacancyController(CrewingContext context)
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

        // GET: Vacancy
        public async Task<IActionResult> Preview()
        {
            var vacancies = context.Vacancies
                .Where(v => v.Workersamount > 0)
                .Include(v => v.Sailorpost)
                .Include(v => v.AgreementnumberNavigation.VesselnumberNavigation.CompanynameNavigation);
            return View(await vacancies.ToListAsync());
        }

        // GET: Vacancy/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || context.Vacancies == null)
            {
                return NotFound();
            }

            var vacancy = await context.Vacancies
                .Include(v => v.Sailorpost)
                .Include(v => v.Requirements)
                .Include(v => v.AgreementnumberNavigation.VesselnumberNavigation.Vesseltype)
                .Include(v => v.AgreementnumberNavigation.VesselnumberNavigation.CompanynameNavigation)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (vacancy == null)
            {
                return NotFound();
            }

            return View(vacancy);
        }

        // GET: Vacancy/Create
        public IActionResult Create()
        {
            ViewData["Agreementnumber"] = new SelectList(context.Agreements, "Agreementnumber", "Agreementnumber");
            ViewData["Sailorpostid"] = new SelectList(context.Sailorposts, "Id", "Id");
            return View();
        }

        // POST: Vacancy/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Agreementnumber,Sailorpostid,Workersamount,Salary,Description,Term")] Vacancy vacancy)
        {
            if (ModelState.IsValid)
            {
                context.Add(vacancy);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Preview));
            }
            ViewData["Agreementnumber"] = new SelectList(context.Agreements, "Agreementnumber", "Agreementnumber", vacancy.Agreementnumber);
            ViewData["Sailorpostid"] = new SelectList(context.Sailorposts, "Id", "Id", vacancy.Sailorpostid);
            return View(vacancy);
        }

        // GET: Vacancy/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.Vacancies == null)
            {
                return NotFound();
            }

            var vacancy = await context.Vacancies.FindAsync(id);
            if (vacancy == null)
            {
                return NotFound();
            }
            ViewData["Agreementnumber"] = new SelectList(context.Agreements, "Agreementnumber", "Agreementnumber", vacancy.Agreementnumber);
            ViewData["Sailorpostid"] = new SelectList(context.Sailorposts, "Id", "Id", vacancy.Sailorpostid);
            return View(vacancy);
        }

        // POST: Vacancy/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Agreementnumber,Sailorpostid,Workersamount,Salary,Description,Term")] Vacancy vacancy)
        {
            if (id != vacancy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(vacancy);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacancyExists(vacancy.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Preview));
            }
            ViewData["Agreementnumber"] = new SelectList(context.Agreements, "Agreementnumber", "Agreementnumber", vacancy.Agreementnumber);
            ViewData["Sailorpostid"] = new SelectList(context.Sailorposts, "Id", "Id", vacancy.Sailorpostid);
            return View(vacancy);
        }

        // GET: Vacancy/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Vacancies == null)
            {
                return NotFound();
            }

            var vacancy = await context.Vacancies
                .Include(v => v.AgreementnumberNavigation)
                .Include(v => v.Sailorpost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vacancy == null)
            {
                return NotFound();
            }

            return View(vacancy);
        }

        // POST: Vacancy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Vacancies == null)
            {
                return Problem("Entity set 'CrewingContext.Vacancies'  is null.");
            }
            var vacancy = await context.Vacancies.FindAsync(id);
            if (vacancy != null)
            {
                context.Vacancies.Remove(vacancy);
            }
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Preview));
        }

        private bool VacancyExists(int id)
        {
            return (context.Vacancies?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeOnRespond(int id)
        {
            if (!User.IsInRole("Client") || !VacancyExists(id))
            {
                return NotFound();
            }

            Client? client = (Client?)await context.FindUserOrNullAsync(User.Identity!.Name!);
            if (client != null)
            {
                client.Status = $"Responded on vacancy with id {id}";
                context.Update(client);
                await context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Preview));
        }
    }
}
