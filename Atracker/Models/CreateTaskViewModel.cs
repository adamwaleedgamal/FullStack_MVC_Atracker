using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atracker.Models
{
    public class CreateTaskViewModel
    {
        [Required] public string Title { get; set; }
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Deadline")]
        public DateTime Deadline { get; set; }

        [Display(Name = "From Location")]
        public string? FromLocation { get; set; }

        [Display(Name = "To Location")]
        public string? ToLocation { get; set; }

        [Required] public TaskPriority Priority { get; set; }

        [Required]
        [Display(Name = "Assign To")]
        public string AssignedToId { get; set; }

        public IEnumerable<SelectListItem> TrackerList { get; set; }
    }
}
