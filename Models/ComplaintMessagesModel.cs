using System;
using System.ComponentModel.DataAnnotations;

namespace SLBFEMS.Models
{
    public class ComplaintMessagesModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ComplaintId { get; set; }

        [Required]
        public string Nic { get; set; }

        [Required]
        [StringLength(int.MaxValue)]
        public string Message { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
