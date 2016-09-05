using Microsoft.AspNet.Identity.EntityFramework;

namespace OrderingService.Persistence
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext() : base("AuthDbContext")
        {
        }
    }
}