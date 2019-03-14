using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Security2.CustomAutt
{
//    public class AuthHandler : SignInAuthenticationHandler<TestAuthOptions>
    public class AuthHandler : AuthenticationHandler<TestAuthOptions>
    {
        private string header = "test";

        /// <inheritdoc />
        public AuthHandler(IOptionsMonitor<TestAuthOptions> options,
            ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <inheritdoc />
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
//            if (Request.Headers["Test"] != "Test")
//                return AuthenticateResult.Fail("Hui");
//            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestName"),
                new Claim(ClaimTypes.Email, "TestName"),
                new Claim(ClaimTypes.Authentication, TestAuthOptions.DefaultScheme)
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrinc = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(claimsPrinc, TestAuthOptions.DefaultScheme);
            return AuthenticateResult.Success(ticket);
//            return AuthenticateResult.Fail("Fuck");
        }

        /// <inheritdoc />
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers.Add("Sasi","hui challenge");
            return base.HandleChallengeAsync(properties);
        }

        /// <inheritdoc />
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.Headers.Add("Sasy","403 hui");
            return base.HandleForbiddenAsync(properties);
        }

//        /// <inheritdoc />
//        protected override async Task HandleSignOutAsync(AuthenticationProperties properties)
//        {
//        }
//
//        /// <inheritdoc />
//        protected override async Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
//        {
//            Response.Headers.Add("Test","Test");
//        }
    }
}