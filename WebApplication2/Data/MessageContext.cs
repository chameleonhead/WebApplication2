using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Data
{
    public class MessageContext : DbContext
    {
        public MessageContext(DbContextOptions<MessageContext> options) : base(options)
        {
        }
        
        public DbSet<Message> Messages { get; set; }
    }
}
