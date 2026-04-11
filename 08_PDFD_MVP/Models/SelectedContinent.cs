namespace VisitorLog_PDFD.Models
{
    public class SelectedContinent
    {
        public int SelectedContinentId { get; set; } // Primary Key
        public int PersonId { get; set; } // Reference to the person (assume you handle authentication elsewhere)
        public int ContinentId { get; set; } // Selected Continent
        public Person? Person { get; set; } // Navigation Property
        public Continent? Continent { get; set; } // Navigation Property
        public Boolean IsDeleted { get; set; }  //Flag to indicate 
    }
}
