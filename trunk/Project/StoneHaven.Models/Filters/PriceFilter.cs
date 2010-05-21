namespace StoneHaven.Models.Filters
{
    public class PriceFilter
    {
        public int CompanyId { get; set; }

        public int ProjectId { get; set; }

        public string PricesFor { get; set; }
    }
}