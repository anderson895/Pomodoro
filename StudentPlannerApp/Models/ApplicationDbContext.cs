using Microsoft.EntityFrameworkCore;
using StudentPlannerApp.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<StudySession> StudySessions { get; set; }
    public DbSet<PomodoroSchedule> Schedules { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
