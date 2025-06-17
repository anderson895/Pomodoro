using System;
using System.ComponentModel.DataAnnotations;

namespace StudentPlannerApp.Models
{
    public class PomodoroSchedule
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Activity { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PomodoroDuration { get; set; }  // Dapat hindi nullable (walang '?')
        public int BreakDuration { get; set; }     // Dapat hindi nullable (walang '?')
    }



}
