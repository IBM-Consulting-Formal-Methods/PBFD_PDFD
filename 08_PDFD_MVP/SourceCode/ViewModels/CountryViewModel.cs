namespace VisitorLog_PDFD.ViewModels
{
    public class CountryViewModel
    {
        public int CountryId { get; set; }              // ID of the country
        public required string CountryName { get; set; }        // Name of the country
        public int SelectedContinentId { get; set; } // The SelectedContinentId mapped to this country
        public required string ContinentName { get; set; }      // The name of the continent for grouping
        public string? NameTypeName { get; set; }       // The name of the NameType
        public bool IsSelected { get; set; }
    }
}
