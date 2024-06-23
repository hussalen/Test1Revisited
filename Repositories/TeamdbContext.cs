using Microsoft.EntityFrameworkCore;

namespace GakkoHorizontalSlice.Repositories;

public class TeamDbContext : DbContext
{
    private readonly string? _connectionString;

    public TeamDbContext() { }

    public TeamDbContext(
        IConfiguration configuration,
        DbContextOptions<TeamDbContext> options
    )
        : base(options)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(
                nameof(configuration),
                "Connection string is not set"
            );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}