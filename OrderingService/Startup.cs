using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using OrderingService;
using OrderingService.Models;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Startup))]

namespace OrderingService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
        }

        public static string PublicClientId { get; private set; }

        private void ConfigureOAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext<AuthDbContext>(() => new AuthDbContext());
            app.CreatePerOwinContext<UserManager<IdentityUser>>(CreateManager);

            PublicClientId = "self";
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/oauth/token"),
                Provider = new AuthorizationServerProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                AllowInsecureHttp = true,

            });
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
        {
            private readonly string _publicClientId;

            public AuthorizationServerProvider(string publicClientId)
            {
                if (publicClientId == null)
                {
                    throw new ArgumentNullException("publicClientId");
                }

                _publicClientId = publicClientId;
            }

            public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
            {
                if (context.ClientId == null)
                {
                    context.Validated();
                }

                return Task.FromResult<object>(null);
            }
            public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
            {
                var userManager = context.OwinContext.GetUserManager<UserManager<IdentityUser>>();
                IdentityUser user = null;
                try
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                }
                catch
                {
                    // Could not retrieve the user due to error.
                    context.SetError("server_error");
                    context.Rejected();
                    return;
                }
                if (user != null)
                {
                    var identity = await userManager.CreateIdentityAsync(
                                                            user,
                                                            DefaultAuthenticationTypes.ExternalBearer);
                    context.Validated(identity);
                }
                else
                {
                    context.SetError("invalid_grant", "Invalid UserId or password'");
                    context.Rejected();
                }
            }
        }

        private static UserManager<IdentityUser> CreateManager(IdentityFactoryOptions<UserManager<IdentityUser>> options, IOwinContext context)
        {
            var userStore = new UserStore<IdentityUser>(context.Get<AuthDbContext>());
            var owinManager = new UserManager<IdentityUser>(userStore);
            return owinManager;
        }
    }
}
