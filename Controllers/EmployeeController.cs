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
    }
}
