using Crewing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Crewing.Controllers
{
    public class EmployeeController : Controller
    {
        private CrewingContext context;
        private IConfiguration configuration;

        public EmployeeController(CrewingContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!User.Identity!.IsAuthenticated || (!User.IsInRole("Manager") && !User.IsInRole("Administrator")))
            {
                context.Result = NotFound();
                return;
            }

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

        // GET: Employee/Statistics
        public async Task<IActionResult> Statistics()
        {
            var managersProductivity = await this.context.Managerslastyearproductivities.ToListAsync();
            return View(managersProductivity);
        }

        // GET: Employee/ClientsAwaitingConfirmation
        public async Task<IActionResult> ClientsAwaitingConfirmation()
        {
            var clients = await context.Clients
                .Where(c => c.Status.StartsWith("Responded on vacancy with id "))
                .ToListAsync();
            ClientsVacancies dictionary = new ClientsVacancies();

            foreach (var client in clients)
            {
                client.Status = String.Concat(client.Status.Where(Char.IsDigit));
                if (!String.IsNullOrEmpty(client.Status))
                {
                    Vacancy? vacancy = await context.Vacancies
                        .Where(v => v.Id == Convert.ToInt32(client.Status))
                        .Include(v => v.Sailorpost)
                        .FirstOrDefaultAsync();
                    if (vacancy != null)
                        dictionary.IdPairs.Add(client.Id, vacancy.Id);
                }
            }
            dictionary.Clients = clients;
            dictionary.Vacancies = await context.Vacancies.ToListAsync();
            return View(dictionary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveClient(int clientId, int vacancyId)
        {
            Employee? employee = await context.Employees.Where(e => e.Email == User.Identity!.Name).FirstOrDefaultAsync();
            Vacancy? vacancy = await context.Vacancies.FindAsync(vacancyId);
            Client? client = await context.Clients.FindAsync(clientId);

            if (client == null || vacancy == null || employee == null)
                return NotFound();

            client.Status = $"Working on vacancy with id {vacancyId}";
            context.Update(client);
            Contract contract = new Contract()
            {
                Clientid = clientId,
                Employeeid = employee.Id,
                Vacancyid = vacancy.Id
            };
            await context.Contracts.AddAsync(contract);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(ClientsAwaitingConfirmation));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefuseClient(int clientId)
        {
            Client? client = await context.Clients.FindAsync(clientId);

            if (client == null)
                return NotFound();

            client.Status = $"Looking for a job";
            context.Update(client);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(ClientsAwaitingConfirmation));
        }
    }
}
