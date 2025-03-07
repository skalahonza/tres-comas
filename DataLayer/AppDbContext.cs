using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

}
