using Microsoft.EntityFrameworkCore;

namespace theWall.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter alongcopy
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Message> messages {get;set;}
        public DbSet<Comment> comments {get;set;}
    }
}