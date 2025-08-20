// In Models/LogType.cs

namespace Atracker.Models
{
    // THIS IS THE NEW ENUM that was missing.
    // It defines the different types of maintenance that can be logged.
    public enum LogType
    {
        Inspection,
        OilChange,
        TireRotation,
        Repair,
        Other
    }
}