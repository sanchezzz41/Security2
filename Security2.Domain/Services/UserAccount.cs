using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Security2.Database;
using Security2.Database.Entities;
using Security2.Domain.Utils;
using Security2.Dto.Models;
using Security2.Gronsfer;

namespace Security2.Domain.Services
{
    public class UserAccount
    {
        private readonly DatabaseContext _userStore;
        private readonly GronsfeldService _gronsfeldService;
        private readonly KeyGenerator _keyGenerator;

        public UserAccount(DatabaseContext userStore, KeyGenerator keyGenerator, GronsfeldService gronsfeldService)
        {
            _userStore = userStore;
            _keyGenerator = keyGenerator;
            _gronsfeldService = gronsfeldService;
        }

        public async Task<Guid> Registration(UserInfo model)
        {
            if (_userStore.Users.Any(x => x.Email == model.Email))
                throw new Exception("Current email is exist");
            var passwordKey = _keyGenerator.GenerateKey();
            
            var resPassword = _gronsfeldService.Encrypt(model.Password, passwordKey);

            var resultUser = new User(model.Email, resPassword, passwordKey, model.Name);
            _userStore.Users.Add(resultUser);
            await _userStore.SaveChangesAsync();
            return resultUser.UserGuid;
        }

        public async Task<Guid> RegistrationWithTimePassword(UserRegisterWithTimePassword model)
        {
            var resultUser = new User(model.Email, null, null, model.Name);
            _userStore.Users.Add(resultUser);
            await _userStore.SaveChangesAsync();
            var date = DateTime.Now.Date;
            var resultList = new List<UserPassword>();
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.FirstPass, date));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.SecondPass, date.AddDays(1)));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.ThreePass, date.AddDays(2)));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.FourPass, date.AddDays(3)));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.FivePass, date.AddDays(4)));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.SixPass, date.AddDays(5)));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.SevenPass, date.AddDays(6)));

            _userStore.UserPasswords.AddRange(resultList);
            await _userStore.SaveChangesAsync();
            return resultUser.UserGuid;
        }

        public User Login(UserLogin loginModel)
        {
            var resUser = _userStore.Users.FirstOrDefault(x => x.Email == loginModel.Email);
            if (resUser == null)
                return null;

            var passwordHash = _gronsfeldService.Encrypt(loginModel.Password, resUser.Key);
            if (passwordHash == resUser.PasswordHash)
                return resUser;

            return null;
        }


        public async Task<List<User>> GetUsers()
        {
            return await _userStore.Users.ToListAsync();
        }

        public async Task<List<UserModel>> GetOriginalUsers()
        {
            var result = new List<UserModel>();
            foreach (var user in _userStore.Users)
            {
                var password = _gronsfeldService.Decrypt(user.PasswordHash, user.Key);
                result.Add(new UserModel(user.Email, password, user.Name, user.PasswordHash, user.Key));
            }

            return result;
        }

        public (User, bool) AuthrorizeWithTimePassword(UserLogin login)
        {
            var user = _userStore.Users.FirstOrDefault(x => x.Email == login.Email);
            if (user == null)
                return (null, false);

            var timePasswords =
                _userStore.UserPasswords.FirstOrDefault(
                    x => x.UserGuid == user.UserGuid
                         && string.CompareOrdinal(x.Password, login.Password) == 0
                         && x.Date == DateTime.Now.Date);

            if (timePasswords != null)
            {
                timePasswords.IsUsed = true;
                _userStore.SaveChanges();
                var isNeedRefreshPasswords = _userStore.UserPasswords.Where(x => x.UserGuid == user.UserGuid)
                    .OrderBy(x => x.Date)
                    .Last();
                return (user, isNeedRefreshPasswords.Date == DateTime.Now.Date);
            }

            return (null, false);
        }
 
        public async Task UpdateTimePasswords(UserRegisterWithTimePassword model)
        {
            var resultUser = await _userStore.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            var isNeedUpdatePass = _userStore.UserPasswords.Where(x => x.UserGuid == resultUser.UserGuid)
                .OrderBy(x => x.Date)
                .Last();
            var date = DateTime.Now.Date.AddDays(1);
            var resultList = new List<UserPassword>();
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.FirstPass, date));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.SecondPass, date.AddDays(1)));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.ThreePass, date.AddDays(2)));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.FourPass, date.AddDays(3)));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.FivePass, date.AddDays(4)));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.SixPass, date.AddDays(5)));
            resultList.Add(new UserPassword(resultUser.UserGuid, model.TimePasswords.SevenPass, date.AddDays(6)));

            _userStore.UserPasswords.AddRange(resultList);
            await _userStore.SaveChangesAsync();
        }
    }
}