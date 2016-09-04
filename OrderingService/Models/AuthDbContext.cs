using Microsoft.AspNet.Identity.EntityFramework;

namespace OrderingService.Models
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext() : base("AuthDbContext")
        {
        }
    }
}