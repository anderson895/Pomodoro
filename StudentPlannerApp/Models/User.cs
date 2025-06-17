using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPlannerApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Fullname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        // Navigation property
        public virtual ICollection<StudySession> StudySessions { get; set; }
    }
}