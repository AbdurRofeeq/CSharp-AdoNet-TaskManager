using System;
namespace Ado.Net_Example.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Tittle { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime DueDate { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool ISCompleted { get; set; }

        public string Status => ISCompleted ? "Completed" : "Pending";

        public string DueInfo
        {
            get
            {
                if (ISCompleted)
                {
                    return "Task is completed.";
                }
                    var timeUntilDue = DueDate - DateTime.Today;
                    if (timeUntilDue.TotalDays < 0)
                    {
                        return "Task is overdue.";
                    }
                    if (timeUntilDue.TotalDays == 0)
                    {
                        return "Task is due today.";
                    }
                    if (timeUntilDue.TotalDays == 1)
                    {
                        return "Task is due tomorrow.";
                    }
                    return $"Task is due in {timeUntilDue.Days} days.";
            }
        }
    }
}