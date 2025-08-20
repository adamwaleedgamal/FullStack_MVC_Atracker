// In Models/TaskItem.cs
using System.ComponentModel.DataAnnotations;

namespace Atracker.Models // <-- Make sure this namespace matches your project
{
    // These enums define the possible options for Priority and Status
    public enum Priority
    {
        Low,
        Medium,
        Urgent
    }

    public enum Status
    {
        Pending,
        [Display(Name = "On The Way")]
        OnTheWay,
        Completed,
        Postponed
    }

    public class TaskItem
    {
        // This will be the primary key in the database table
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }

        public string Location { get; set; }

        public Priority DeliveryPriority { get; set; }

        public Status TaskStatus { get; set; } = Status.Pending; // Default status is Pending
        public string? ReplyMessage { get; set; }
        public string? Feedback { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}