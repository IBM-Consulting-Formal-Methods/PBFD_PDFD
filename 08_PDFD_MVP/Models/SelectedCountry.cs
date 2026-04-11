namespace VisitorLog_PDFD.Models
{
    public class SelectedCountry
    {
        public int SelectedCountryId { get; set; } // Primary Key
        public int SelectedContinentId { get; set; } // Foreign Key to SelectedContinent
        public int CountryId { get; set; } // Foreign Key to Countries

        public SelectedContinent? SelectedContinent { get; set; }
        public Country? Country { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}
