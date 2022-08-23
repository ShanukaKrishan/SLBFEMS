using System.ComponentModel.DataAnnotations;

namespace SLBFEMS.Models
{
    public class EducationDataModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NIC { get; set; }

        [Required]
        public string Start { get; set; }

        public string End { get; set; } = "Present";

        [Required]
        public string Name { get; set; }

        public bool IsDelete { get; set; } = false;
    }
}
