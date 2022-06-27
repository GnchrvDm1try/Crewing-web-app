namespace Crewing.Models
{
    public class ClientsVacancies
    {
        public List<Client> Clients { get; set; } = null!;
        public List<Vacancy> Vacancies { get; set; } = null!;
        public Dictionary<int, int> IdPairs { get; set; } = new Dictionary<int, int>();
    }
}
