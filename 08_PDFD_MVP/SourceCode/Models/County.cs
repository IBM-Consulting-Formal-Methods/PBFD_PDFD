using VisitorLog_PDFD.Models;
using System.ComponentModel.DataAnnotations;

namespace VisitorLog_PDFD.Models
{
    public class County
    {
        public int CountyId { get; set; }
        public required string Name { get; set; }
        public int StateId { get; set; }
        public virtual State? State { get; set; }
        public virtual ICollection<City>? Cities { get; set; }
        public int NameTypeId { get; set; } // Foreign Key
        public NameType? NameType { get; set; } // Navigation property

    }
}
