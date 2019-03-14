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
            if(_userStore.Users.Any(x=>x.Email==model.Email))
                throw new Exception("Current email is exist");
            var passwordKey = _keyGenerator.GenerateKey();

            var resPassword = _gronsfeldService.Encrypt(model.Password, passwordKey);

            var resultUser = new User(model.Email, resPassword, passwordKey, model.Name);
            _userStore.Users.Add(resultUser);
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
    }
}