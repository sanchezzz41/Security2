using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Security2.Domain.Models;
using Security2.WebClient.RefitClients;
using Security2.WebClient.Services;

namespace Security2.WebClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userClient;

        public UserController(UserService userClient)
        {
            _userClient = userClient;
        }

        [HttpPost("login")]
        public async Task Authorize(UserLogin info)
        {
            await _userClient.Login(info);
        }
    }
}
