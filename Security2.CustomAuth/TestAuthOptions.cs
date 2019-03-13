using Microsoft.AspNetCore.Authentication;

namespace Security2.CustomAuth
{
    public class TestAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "MyScheme";
    }
}