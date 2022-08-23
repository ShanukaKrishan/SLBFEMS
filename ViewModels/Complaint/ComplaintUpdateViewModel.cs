using System.ComponentModel.DataAnnotations;

namespace SLBFEMS.ViewModels.Complaint
{
    public class ComplaintUpdateViewModel: ComplaintCreateViewModel
    {
        [Required(ErrorMessage = "Compleation status is required")]
        public bool IsComplete { get; set; }
    }
}
