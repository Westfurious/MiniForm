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
}