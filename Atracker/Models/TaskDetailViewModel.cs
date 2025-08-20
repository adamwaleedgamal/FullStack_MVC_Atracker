namespace Atracker.Models
{
    public class TaskDetailViewModel
    {
        public TaskAssignment Task { get; set; }
        public string AssignedByName { get; set; }
        public UpdateTaskViewModel UpdateForm { get; set; }
    }
}
