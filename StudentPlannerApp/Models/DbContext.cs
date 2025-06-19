using Microsoft.EntityFrameworkCore;
using StudentPlannerApp.Models;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<StudySession> StudySessions { get; set; }
    public DbSet<PomodoroSchedule> Schedules { get; set; }

    public DbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }
}
