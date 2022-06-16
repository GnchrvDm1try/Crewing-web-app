namespace Crewing.Models
{
    public class ReviewCreationModel
    {
        public string Comment { get; set; } = null!;
        public double Estimation { get; set; }
        public string Companyname { get; set; } = null!;
    }
}
