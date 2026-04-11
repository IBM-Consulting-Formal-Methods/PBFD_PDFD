namespace VisitorLog_PDFD.Models
{
    public class SelectedState
    {
        public int SelectedStateId { get; set; }
        public int SelectedCountryId { get; set; }
        public int StateId { get; set; }

        public SelectedCountry? SelectedCountry { get; set; }
        public State? State { get; set; }
        public Boolean IsDeleted { get; set; }
    }

}
