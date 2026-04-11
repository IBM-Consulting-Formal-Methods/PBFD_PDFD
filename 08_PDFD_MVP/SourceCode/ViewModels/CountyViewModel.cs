namespace VisitorLog_PDFD.ViewModels
{
    public class CountyViewModel
    {
        public int CountyId { get; set; }              // ID of the county
        public required string CountyName { get; set; }        // Name of the county
        public int SelectedStateId { get; set; } // The SelectedStateId mapped to this country
        public required string StateName { get; set; }      // The name of the state for grouping
        public string? NameTypeName { get; set; }       // The name of the NameType
        public bool IsSelected { get; set; }    // The country from previous selections

    }
}
