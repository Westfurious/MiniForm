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
    }
}