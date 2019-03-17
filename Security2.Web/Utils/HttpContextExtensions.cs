using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Security2.Web.Utils
{
    public static class HttpContextExtensions
    {
        public static string GetEmail(this HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Email).Value;
        }
    }
}
