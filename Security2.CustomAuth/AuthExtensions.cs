using System;
using Microsoft.AspNetCore.Authentication;

namespace Security2.CustomAutt
{
    public static class AuthExtensions
    {
        public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder, Action<TestAuthOptions> configureOptions)
        {
            return builder.AddScheme<TestAuthOptions, AuthHandler>(TestAuthOptions.DefaultScheme, "MyDisplayNameScheme",
                configureOptions);
        }
    }
}