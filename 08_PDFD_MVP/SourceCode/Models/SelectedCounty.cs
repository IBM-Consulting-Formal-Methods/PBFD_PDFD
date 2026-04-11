namespace VisitorLog_PDFD.Models
{
    public class SelectedCounty
    {
        public int SelectedCountyId { get; set; }
        public int SelectedStateId { get; set; }
        public int CountyId { get; set; }

        public SelectedState? SelectedState { get; set; }
        public County? County { get; set; }

        public Boolean IsDeleted { get; set; }
    }
}
