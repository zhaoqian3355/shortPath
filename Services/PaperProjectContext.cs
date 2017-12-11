
using Microsoft.EntityFrameworkCore;
using shortPath;

namespace shortPath
{
    public class PaperProjectContext:DbContext
    {
        public DbSet<City> City { get; set; }
        public DbSet<Train> Train{get;set;}
        public DbSet<Flight> Flight { get; set; }
        public DbSet<TrainStation> TrainStation { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=PaperProject.db");
        }
    }
}