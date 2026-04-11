namespace VisitorLog_PDFD.Models
{
    public class Continent
    {
        public int ContinentId { get; set; }
        public required string Name { get; set; }
        public ICollection<Country>? Countries { get; set; } = new List<Country>();
        public int NameTypeId { get; set; } // Foreign Key
        public NameType? NameType { get; set; } // Navigation property
    }

}
