using Microsoft.EntityFrameworkCore;
using PostWork.Models;

namespace PostWork.Data
{
    public class SubmissionContext : DbContext
    {
        public SubmissionContext(DbContextOptions<SubmissionContext> options) : base(options) { }
    }
}