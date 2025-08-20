// In Models/DailyReport.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Atracker.Models
{
    public class DailyReport
    {
        public int Id { get; set; }
        public string ReportContent { get; set; }
        public DateTime ReportDate { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}