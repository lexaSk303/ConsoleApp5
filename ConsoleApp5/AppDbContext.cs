using ConsoleApp5.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp5;

class AppDbContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Group> Groups => Set<Group>();
    public AppDbContext(DbContextOptions opt) : base(opt)
    {

    }
}
