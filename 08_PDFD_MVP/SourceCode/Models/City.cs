namespace VisitorLog_PDFD.Models
{
    public class City
    {
        public int CityId { get; set; }
        public required string Name { get; set; }
        public int CountyId { get; set; }
        public County? County { get; set; }
        public int NameTypeId { get; set; } // Foreign Key
        public NameType? NameType { get; set; } // Navigation property
    }

}
