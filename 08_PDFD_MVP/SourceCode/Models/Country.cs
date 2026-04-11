namespace VisitorLog_PDFD.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public required string Name { get; set; }
        public int ContinentId { get; set; }
        public Continent? Continent { get; set; }
        public ICollection<State> States { get; set; } = new List<State>();
        public int NameTypeId { get; set; } // Foreign Key
        public NameType? NameType { get; set; } // Navigation property

    }

}
