using Microsoft.EntityFrameworkCore;
using PostWork.Models;

namespace PostWork.Data
{
    public class PostContext : DbContext
    {
        public PostContext(DbContextOptions<PostContext> options) : base(options) { }

        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}