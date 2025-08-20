using System.ComponentModel.DataAnnotations;

namespace Atracker.Models
{
    public class UpdateTaskViewModel
    {
        public int TaskId { get; set; }

        [Required]
        [Display(Name = "New Status")]
        public AssignmentStatus NewStatus { get; set; }

        [Display(Name = "Reply / Comment")]
        [StringLength(500)]
        public string? ReplyText { get; set; }
    }
}
