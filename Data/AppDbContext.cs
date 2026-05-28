using Microsoft.EntityFrameworkCore;
using MiniForm.Models;

namespace MiniForm.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Form> Forms => Set<Form>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Submission> Submissions => Set<Submission>();

    public DbSet<Answer> Answers => Set<Answer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(user => user.Email)
                .HasMaxLength(256);

            entity.HasIndex(user => user.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasOne(form => form.CreatedByUser)
                .WithMany()
                .HasForeignKey(form => form.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasOne(s => s.Form)
                .WithMany()
                .HasForeignKey(s => s.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasOne(a => a.Submission)
                .WithMany(s => s.Answers)
                .HasForeignKey(a => a.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}