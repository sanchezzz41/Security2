using System.Threading.Tasks;
using Refit;
using Security2.Domain.Models;

namespace Security2.WebClient.RefitClients
{
    public interface IUserClient
    {
        [Post("/login")]
        Task<string> Login([Body] UserLogin model);
    }
}
