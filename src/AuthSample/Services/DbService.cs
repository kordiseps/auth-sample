using System;
using System.Data;
using System.Linq;
using AuthSample.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace AuthSample.Services
{
    public interface IDbService
    {
        string NewSession(int userId);
        int GetUserIdFromSession(string sessionId);
        Account GetAccountByUserName(string userName);
        int InsertAccount(Account account);
        void InsertRole(Role role);
        bool IsInRole(int userId, string path);
        void UpdateUserPassword(string username, string passwordHash, string salt);
        void UpdateLastLogin(string userName);
    }

    public class DbService : IDbService
    {
        private readonly IConfiguration _configuration;

        public DbService(IConfiguration configuration)
        {
            _configuration = configuration;
            Setup();
        }
        void Setup()
        {
            using var connection = GetConnection();
 
            var table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name in ('Account','Role', 'Session');");
            if (table.Count()==3)
            {
                return;
            }

            var crateTable = @"
CREATE TABLE Account (
     InsertedAt DATETIME NOT NULL, 
     DisplayName TEXT NOT NULL, 
     UserName TEXT NOT NULL, 
     LastLogin DATETIME NULL, 
     PasswordHash TEXT NOT NULL, 
     PasswordSalt TEXT NOT NULL, 
     Blocked INTEGER NOT NULL 
);
CREATE TABLE Role (
    InsertedAt DATETIME NOT NULL, 
    Path TEXT NOT NULL, 
    UserId INTEGER NOT NULL 
);
CREATE TABLE Session (
    InsertedAt DATETIME NOT NULL, 
    ExpiresAt DATETIME NOT NULL, 
    SessionId TEXT NOT NULL, 
    UserId INTEGER NOT NULL 
);
";     
            connection.Execute(crateTable);
            
            var _passwordService = new PasswordService();
            var salt = _passwordService.GenerateSalt();
            var hash = _passwordService.HashText("1234", salt);
            
            var user = new Account
            {
                Blocked = false,
                DisplayName = "admin user",
                LastLogin = DateTime.Now,
                UserName = "admin",
                PasswordSalt = Convert.ToBase64String(salt),
                PasswordHash = hash
            };

            var userId = InsertAccount(user);

            InsertRole(new Role
            {
                UserId = userId, 
                Path = "/WeatherForecast"
            });
            InsertRole(new Role
            {
                UserId = userId, 
                Path = "/Admin/MakeUserAdmin"
            });
            InsertRole(new Role
            {
                UserId = userId, 
                Path = "/Admin/ResetUserPassword"
            });
        }
        private IDbConnection GetConnection()
        {
            var connectionString = _configuration.GetConnectionString("default");
            var connection = new SqliteConnection(connectionString);
            return connection;
        }

        public bool IsInRole(int userId, string path)
        {
            using var connection = GetConnection();
            var role = connection
                .Query<Role>("SELECT * FROM Role WHERE UserId = @UserId and Path = @Path", new { Path = path,UserId= userId })
                .FirstOrDefault();
            return role is null;
        }
        public string NewSession(int userId)
        {
            var expiresIn = _configuration.GetValue<int>("SessionExpiresIn");
            var sessionId = Guid.NewGuid().ToString().Substring(0, 8).ToLower();
            var s = new Session
            {
                UserId = userId,
                SessionId = sessionId,
                InsertedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(expiresIn)
            };
            var sql = "INSERT INTO Session (UserId, SessionId,InsertedAt,ExpiresAt) VALUES (@UserId, @SessionId,@InsertedAt,@ExpiresAt) ";
            using var connection = GetConnection();
            connection.Execute(sql, s);
            return sessionId;
        }

        public int GetUserIdFromSession(string sessionId)
        { 
            using var connection = GetConnection();
            var session = connection
                .Query<Session>("SELECT rowid as Id,* FROM Session WHERE SessionId = @SessionId", new { SessionId = sessionId })
                .FirstOrDefault();
            if (session is null)
            {
                return -1;
            }

            if (session.ExpiresAt< DateTime.Now)
            {
                connection.Execute("DELETE FROM Session WHERE rowid= @Id", new { Id = session.Id });
                return -1;
            }        
            return session.UserId;
        }

        public void UpdateLastLogin(string userName)
        {
            using var connection = GetConnection();
            connection.Execute("UPDATE Account SET LastLogin= @LastLogin WHERE UserName=@UserName", 
                new
                {
                    LastLogin = DateTime.Now,
                    UserName = userName
                });
        }
        public Account GetAccountByUserName(string userName)
        {
            using var connection = GetConnection();
            var user = connection
                .Query<Account>("SELECT rowid as Id,* FROM Account WHERE UserName = @UserName", new { UserName = userName })
                .FirstOrDefault();
            return user;
        }

        public int InsertAccount(Account account)
        {
            using var connection = GetConnection();
            var sql =
                "INSERT INTO Account (Blocked, InsertedAt, DisplayName, LastLogin, UserName, PasswordSalt, PasswordHash) " +
                "VALUES (@Blocked, @InsertedAt, @DisplayName, @LastLogin, @UserName, @PasswordSalt, @PasswordHash);" +
                "SELECT last_insert_rowid()";
            return connection.Query<int>(sql, account).FirstOrDefault();
        }
        
        public void InsertRole(Role role)
        {
            using var connection = GetConnection();
            connection.Execute("INSERT INTO Role (InsertedAt, UserId, Path) VALUES (@InsertedAt, @UserId, @Path)",
                role);
        }

        public void UpdateUserPassword(string username, string passwordHash, string salt)
        {
            using var connection = GetConnection();
            connection.Execute("UPDATE Account SET PasswordSalt= @PasswordSalt, PasswordHash= @PasswordHash WHERE UserName=@UserName", 
                new
                {
                    PasswordSalt = salt,
                    PasswordHash = passwordHash,
                    UserName = username
                });

        }
    }
}