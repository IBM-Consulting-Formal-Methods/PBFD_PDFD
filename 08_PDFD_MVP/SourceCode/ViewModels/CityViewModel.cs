namespace VisitorLog_PDFD.ViewModels
{
    public class CityViewModel
    {
        public int CityId { get; set; }              // ID of the city
        public required string CityName { get; set; }        // Name of the city
        public int SelectedCountyId { get; set; } // The SelectedCountyId mapped to this county
        public required string CountyName { get; set; }      // The name of the county for grouping
        public string? NameTypeName { get; set; }       // The name of the NameType
        public bool IsSelected { get; set; }    // The country from previous selections
    }
}
