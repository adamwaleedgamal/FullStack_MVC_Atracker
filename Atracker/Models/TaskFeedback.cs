using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atracker.Models
{
    public class TaskFeedback
    {
        [Key] public int Id { get; set; }

        [Required] public int TaskId { get; set; }
        [ForeignKey(nameof(TaskId))] public TaskAssignment Task { get; set; }

        [Required, StringLength(500)]
        public string Content { get; set; }

        [Required] public string SubmittedById { get; set; }
        [ForeignKey(nameof(SubmittedById))] public ApplicationUser SubmittedBy { get; set; }

        [Required] public DateTime SubmittedDate { get; set; }
    }
}
