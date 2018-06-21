using Microsoft.EntityFrameworkCore;
 
namespace beltExam.Models
{
    public class LoginContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public LoginContext(DbContextOptions<LoginContext> options) : base(options) { }

        public DbSet<User> users { get; set; }
        public DbSet<Auction> auction { get; set; }
        public DbSet<Bidder> bidder { get; set; }
    }
}