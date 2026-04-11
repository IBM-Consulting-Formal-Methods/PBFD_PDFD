namespace VisitorLog_PDFD.Models
{
    public class SelectedCity
    {
        public int SelectedCityId { get; set; }
        public int SelectedCountyId { get; set; }
        public int CityId { get; set; }

        public SelectedCounty? SelectedCounty { get; set; }
        public City? City { get; set; }

        public Boolean IsDeleted { get; set; }
    }
}
