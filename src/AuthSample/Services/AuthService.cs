using System;
using AuthSample.Models;

namespace AuthSample.Services
{
    public interface IAuthService
    {
        string Login(string userName, string password);
        void Register(string userName, string password, string displayName);
        bool DoesHavePermission(string sessionId, string path);
        void ResetUserPassword(string userName);
        void MakeUserAdmin(string userName);
    }
    
    public class AuthService: IAuthService
    {
        private readonly IPasswordService _passwordService;
        private readonly IDbService _dbService;

        public AuthService(IPasswordService passwordService, IDbService dbService)
        {
            _passwordService = passwordService;
            _dbService = dbService;
        }

        public bool DoesHavePermission(string sessionId, string path)
        {
            var userId = _dbService.GetUserIdFromSession(sessionId);
            if (userId==-1)
            {
                return false;
            }
            return _dbService.IsInRole(userId, path);
        }

        public void ResetUserPassword(string userName)
        {
            var user = _dbService.GetAccountByUserName(userName);
            if (user is null)
            {
                throw new Exception("User does not exist.");
            }
            
            var salt = _passwordService.GenerateSalt();
            var hash = _passwordService.HashText("123456", salt);
            
            _dbService.UpdateUserPassword(userName,hash,Convert.ToBase64String(salt));;
        }

        public void MakeUserAdmin(string userName)
        {
            var user = _dbService.GetAccountByUserName(userName);
            if (user is null)
            {
                throw new Exception("User does not exist.");
            }

            var paths = new[] { "/Admin/MakeUserAdmin", "/Admin/ResetUserPassword"};
            foreach (var path in paths) 
            {
                if (!_dbService.IsInRole(user.Id, path))
                {
                    _dbService.InsertRole(new Role
                    {
                        UserId = user.Id, 
                        Path = path
                    });
                }
            }
        }

        public string Login(string userName, string password)
        {
            var user = _dbService.GetAccountByUserName(userName);
            if (user is null)
            {
                throw new Exception("User does not exist.");
            }

            if (user.Blocked)
            {
                throw new Exception("User is blocked.");
            }

            var passwordHash = _passwordService.HashText(password, Convert.FromBase64String(user.PasswordSalt));

            if (passwordHash != user.PasswordHash)
            {
                throw new Exception("Password is not correct");
            }
            _dbService.UpdateLastLogin(user.UserName);
            return _dbService.NewSession(user.Id);
        }

        public void Register(string userName, string password, string displayName)
        {
            var user = _dbService.GetAccountByUserName(userName);
            if (user != null)
            {
                throw new Exception("User exists.");
            }

            var salt = _passwordService.GenerateSalt();
            var hash = _passwordService.HashText(password, salt);

            user = new Account
            {
                Blocked = false,
                DisplayName = displayName,
                LastLogin = DateTime.Now,
                UserName = userName,
                PasswordSalt = Convert.ToBase64String(salt),
                PasswordHash = hash
            };

            var userId = _dbService.InsertAccount(user);

            _dbService.InsertRole(new Role
            {
                UserId = userId, 
                Path = "/WeatherForecast"
            });
        }

 
    }


    
}