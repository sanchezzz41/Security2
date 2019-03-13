using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Security2.CustomAuth
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