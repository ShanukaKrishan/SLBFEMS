using System.ComponentModel.DataAnnotations;

namespace SLBFEMS.Models
{
    public class QualificationsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NIC { get; set; }

        [Required]
        public string Qualification { get; set; }

        public bool IsDelete { get; set; } = false;
    }
}
