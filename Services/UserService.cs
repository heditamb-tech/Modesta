using BC = BCrypt.Net.BCrypt;
using Modesta.Data;
using Modesta.Models;
using System;
using System.Linq;

namespace Modesta.Services
{
    public class UserService
    {
        private readonly ModestDbContext _db;

        public UserService()
        {
            _db = new ModestDbContext();
        }

        public bool Register(string username, string email, string password)
        {
            if (_db.Users.Any(u => u.Email == email))
                return false;

            var user = new User
            {
                Username = username,
                Email = email.Trim().ToLower(),
                PasswordHash = BC.HashPassword(password),
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return true;
        }

        public User Login(string email, string password)
        {
            email = email.Trim().ToLower();

            var user = _db.Users.FirstOrDefault(u => u.Email == email);

            System.Diagnostics.Debug.WriteLine($"INPUT EMAIL: {email}");

            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine("USER NOT FOUND");
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"FOUND USER: {user.Email}");

            bool pwOk = BC.Verify(password, user.PasswordHash);

            System.Diagnostics.Debug.WriteLine($"PASSWORD OK: {pwOk}");

            if (!pwOk)
                return null;

            return user;
        }
    }
}