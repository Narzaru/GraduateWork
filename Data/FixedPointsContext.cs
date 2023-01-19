using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class FixedPointsContext : DbContext
{
    public DbSet<Point>? Points { get; set; }
    public DbSet<PointsSet>? PointsSet { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=points.db");
    }
}