using Microsoft.AspNetCore.Authentication;

namespace Security2.CustomAutt
{
    public class TestAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "MyScheme";
    }
}