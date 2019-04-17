using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Security2.Domain.Services;
using Security2.Dto.Models;
using Security2.Rsa;
using Security2.Web.Utils;

namespace Security2.Web.Controllers
{
    [ApiController, Route("[controller]")]
    public class TimePasswordController : Controller
    {
        private readonly UserAccount _userAccount;
        private readonly RsaServerKeys _rsaServerKeys;
        private readonly RsaService _rsaService;
        private readonly IMemoryCache _memoryCache;

        public TimePasswordController(UserAccount userAccount,
            RsaServerKeys rsaServerKeys,
            RsaService rsaService,
            IMemoryCache memoryCache)
        {
            _userAccount = userAccount;
            _rsaServerKeys = rsaServerKeys;
            _rsaService = rsaService;
            _memoryCache = memoryCache;
        }

        [HttpPost("Register")]
        public async Task<Guid> RegisterWithTimePasswords(UserRegisterWithTimePassword model)
        {
            model.TimePasswords.FirstPass = _rsaService.Decrypt<string>(model.TimePasswords.FirstPass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.SecondPass = _rsaService.Decrypt<string>(model.TimePasswords.SecondPass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.ThreePass = _rsaService.Decrypt<string>(model.TimePasswords.ThreePass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.FourPass = _rsaService.Decrypt<string>(model.TimePasswords.FourPass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.FivePass = _rsaService.Decrypt<string>(model.TimePasswords.FivePass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.SixPass = _rsaService.Decrypt<string>(model.TimePasswords.SixPass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.SevenPass = _rsaService.Decrypt<string>(model.TimePasswords.SevenPass, _rsaServerKeys.PrivateKey);
            
            model.Email = _rsaService.Decrypt<string>(model.Email, _rsaServerKeys.PrivateKey);
            model.Name = _rsaService.Decrypt<string>(model.Name, _rsaServerKeys.PrivateKey);
            return await _userAccount.RegistrationWithTimePassword(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>Если true, то надо обновить пароли</returns>
        /// <exception cref="NullReferenceException"></exception>
        [HttpPost("Login")]
        public async Task<bool> Login(UserLogin loginModel)
        {
            loginModel.Email = _rsaService.Decrypt<string>(loginModel.Email, _rsaServerKeys.PrivateKey);
            loginModel.Password = _rsaService.Decrypt<string>(loginModel.Password, _rsaServerKeys.PrivateKey);
            var result = _userAccount.AuthrorizeWithTimePassword(loginModel);
            if(result.Item1==null)
                throw new NullReferenceException("Введенны неверные данные.");
            var user = result.Item1;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            return result.Item2;
        }

        [HttpPost("UpdatePasswords"), Authorize]
        public async Task UpdatePasswords(UserRegisterWithTimePassword model)
        {
            model.Email = _rsaService.Decrypt<string>(model.Email, _rsaServerKeys.PrivateKey);
            if(HttpContext.GetEmail()!=model.Email)
                throw new Exception("Вы не можете поменять пароли данному пользователю");
            model.TimePasswords.FirstPass = _rsaService.Decrypt<string>(model.TimePasswords.FirstPass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.SecondPass = _rsaService.Decrypt<string>(model.TimePasswords.SecondPass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.ThreePass = _rsaService.Decrypt<string>(model.TimePasswords.ThreePass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.FourPass = _rsaService.Decrypt<string>(model.TimePasswords.FourPass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.FivePass = _rsaService.Decrypt<string>(model.TimePasswords.FivePass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.SixPass = _rsaService.Decrypt<string>(model.TimePasswords.SixPass, _rsaServerKeys.PrivateKey);
            model.TimePasswords.SevenPass = _rsaService.Decrypt<string>(model.TimePasswords.SevenPass, _rsaServerKeys.PrivateKey);
            
            model.Name = _rsaService.Decrypt<string>(model.Name, _rsaServerKeys.PrivateKey);
            await _userAccount.UpdateTimePasswords(model);
        }
    }
}