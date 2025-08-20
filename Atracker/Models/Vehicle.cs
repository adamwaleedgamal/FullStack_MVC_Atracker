// In Models/Vehicle.cs
using System.Collections.Generic; // <-- Add this for ICollection
using System.ComponentModel.DataAnnotations;

namespace Atracker.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string VehicleIdentifier { get; set; }
        public string CarType { get; set; } = "Truck";
        public string LicensePlate { get; set; }
        public string? AssignedDriverId { get; set; }
        public ApplicationUser? AssignedDriver { get; set; }
        public DateTime LastInspection { get; set; }
        public DateTime LastOilChange { get; set; }
        public DateTime NextOilChange { get; set; }

        // THIS IS THE MISSING PROPERTY that caused the error
        public ICollection<MaintenanceLog> MaintenanceLogs { get; set; } = new List<MaintenanceLog>();
    }
}