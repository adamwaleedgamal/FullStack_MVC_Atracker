using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atracker.Models
{
    public enum TaskPriority { Normal, Urgent }
    public enum AssignmentStatus { Assigned, OnTheWay, Completed, Postponed }

    public class TaskAssignment
    {
        [Key] public int Id { get; set; }

        [Required] public string Title { get; set; }
        public string? Description { get; set; }

        [Required] public DateTime CreatedDate { get; set; }
        [Required] public DateTime Deadline { get; set; }

        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }

        [Required] public TaskPriority Priority { get; set; }
        [Required] public AssignmentStatus Status { get; set; }

        public string AssignedToId { get; set; }
        [ForeignKey(nameof(AssignedToId))] public virtual ApplicationUser AssignedTo { get; set; }

        public string AssignedById { get; set; }
        [ForeignKey(nameof(AssignedById))] public virtual ApplicationUser AssignedBy { get; set; }

        public int? VehicleId { get; set; }
        [ForeignKey(nameof(VehicleId))] public virtual Vehicle? Vehicle { get; set; }
    }
}
