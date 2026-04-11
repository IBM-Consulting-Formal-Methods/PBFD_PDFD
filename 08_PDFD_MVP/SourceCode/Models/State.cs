namespace VisitorLog_PDFD.Models
{
    public class State
    {
        public int StateId { get; set; }
        public required string Name { get; set; }
        public int CountryId { get; set; }
        public Country? Country { get; set; }
        public int NameTypeId { get; set; } // Foreign Key
        public NameType? NameType { get; set; } // Navigation property

    }

}
