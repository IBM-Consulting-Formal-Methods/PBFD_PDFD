namespace VisitorLog_PDFD.ViewModels
{
    internal class StateViewModel
    {
        public int StateId { get; set; }              // ID of the state
        public required string StateName { get; set; }        // Name of the country
        public int SelectedCountryId { get; set; } // The SelectedCountryId mapped to this country
        public required string CountryName { get; set; }      // The name of the country for grouping
        public string? NameTypeName { get; set; }       // The name of the NameType
        public bool IsSelected { get; set; }    // The country from previous selections

    }
}