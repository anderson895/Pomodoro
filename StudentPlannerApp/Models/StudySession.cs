using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPlannerApp.Models
{
    public class StudySession
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [StringLength(255)]
        public string Topic { get; set; }

        // Foreign key
        public int UserId { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}