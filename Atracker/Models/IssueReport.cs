// In Models/IssueReport.cs

using System;
using System.ComponentModel.DataAnnotations; // <-- Add this for validation attributes

namespace Atracker.Models
{
    public class IssueReport
    {
        public int Id { get; set; }

        // THIS IS THE CRITICAL CHANGE
        // This attribute ensures the user cannot submit an empty form field.
        [Required(ErrorMessage = "Please enter details for your report.")]
        public string Content { get; set; }

        public DateTime ReportDate { get; set; }

        public string ReportedById { get; set; }
        public ApplicationUser ReportedBy { get; set; }
    }
}