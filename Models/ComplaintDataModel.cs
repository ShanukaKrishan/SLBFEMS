using SLBFEMS.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SLBFEMS.Models
{
    public class ComplaintDataModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ComplaintStatus Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? CompletedAt { get; set; } = null;
    }
}
