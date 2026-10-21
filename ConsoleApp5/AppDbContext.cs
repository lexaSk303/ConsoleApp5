using ConsoleApp5.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp5;

class AppDbContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    public AppDbContext(DbContextOptions opt) : base(opt)
    {

    }
}
