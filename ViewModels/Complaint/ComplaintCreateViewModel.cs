using System.ComponentModel.DataAnnotations;

namespace SLBFEMS.ViewModels.Complaint
{
    public class ComplaintCreateViewModel
    {
        [Required(ErrorMessage = "Complaint is Required")]
        public string Complaint { get; set; }
    }
}
