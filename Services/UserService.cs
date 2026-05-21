using System;
using BC = BCrypt.Net.BCrypt;
using Modesta.Data;
using Modesta.Models;
using System.Linq;

namespace Modesta.Services
{
    public class UserService
    {
        private ModestDbContext _db = new ModestDbContext();

        public bool Register(string username, string email, string password)
        {
            if (_db.Users.Any(u => u.Email == email)) return false;
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BC.HashPassword(password),
                CreatedAt = DateTime.Now
            };
            _db.Users.Add(user);
            _db.SaveChanges();
            return true;
        }

        public User Login(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return null;
            if (!BC.Verify(password, user.PasswordHash)) return null;
            return user;
        }

        public bool ChangePassword(int userId, string newPassword)
        {
            var user = _db.Users.Find(userId);
            if (user == null) return false;
            user.PasswordHash = BC.HashPassword(newPassword);
            _db.SaveChanges();
            return true;
        }

        public bool DeleteAccount(int userId)
        {
            var user = _db.Users.Find(userId);
            if (user == null) return false;
            _db.Users.Remove(user);
            _db.SaveChanges();
            return true;
        }

        public bool UpdateProfile(int userId, string bio, string picturePath, string username)
        {
            var user = _db.Users.Find(userId);
            if (user == null) return false;
            if (bio != null) user.Bio = bio;
            if (picturePath != null) user.ProfilePicturePath = picturePath;
            if (username != null) user.Username = username;
            _db.SaveChanges();
            return true;
        }
    }
}