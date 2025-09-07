namespace PoeCurrencyTracker.Api.Models
{
    public class EconomyEndpoint
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string OverviewName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} (ID: {Id}, Overview: {OverviewName})";
        }
    }
}
