// In Models/MaintenanceLog.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace Atracker.Models
{
    public class MaintenanceLog
    {
        public int Id { get; set; }

        // THIS PROPERTY WAS MISSING. It stores the type of maintenance.
        public LogType LogType { get; set; }

        [DataType(DataType.Date)]
        public DateTime DatePerformed { get; set; } // Corrected property name

        public string Notes { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NextDueDate { get; set; } // Corrected property name, nullable

        // Foreign key to link this log to a specific Vehicle
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}