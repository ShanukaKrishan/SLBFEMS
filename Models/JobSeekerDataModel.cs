using SLBFEMS.Enums;
using System.ComponentModel.DataAnnotations;

namespace SLBFEMS.Models
{
    public class JobSeekerDataModel
    {
        [Key]
        public string NIC { get; set; }

        [Required]
        public string CurrentLat { get; set; }

        [Required]
        public string CurrentLong { get; set; }

        [Required]
        public string Profession { get; set; }

        [Required]
        public string CvFileName { get; set; }

        public string BirthCertificateFileName { get; set; } = string.Empty;

        public string PassportFileName { get; set; } = string.Empty;

        public FileVerificationStatus IsCvValidated { get; set; } = FileVerificationStatus.pending;

        public FileVerificationStatus IsBirthCertificateValidated { get; set; } = FileVerificationStatus.pending;

        public FileVerificationStatus IsParsportValidated { get; set; } = FileVerificationStatus.pending;

        public bool IsDelete { get; set; } = false;

    }
}
