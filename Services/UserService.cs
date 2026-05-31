using System;
using System.Linq;
using System.Windows;
using BC = BCrypt.Net.BCrypt;
using Modesta.Models;
using Microsoft.EntityFrameworkCore;

namespace Modesta.Services
{
    public class UserService
    {
        private static string _connString = @"Data Source=C:\Users\Heidi\modesta.db";

        private Modesta.Data.ModestDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<Modesta.Data.ModestDbContext>()
                .UseSqlite(_connString)
                .Options;
            return new Modesta.Data.ModestDbContext(options);
        }

        public bool Register(string username, string email, string password)
        {
            try
            {
                using (var db = GetDb())
                {
                    
                    if (db.Users.Any(u => u.Email == email))
                    {
                        MessageBox.Show("Email al in gebruik");
                        return false;
                    }
                    var user = new User
                    {
                        Username = username,
                        Email = email,
                        PasswordHash = BC.HashPassword(password),
                        CreatedAt = DateTime.Now
                    };

                    if (!db.Users.Any()) user.IsAdmin = true;

                    db.Users.Add(user);
                    db.SaveChanges();
                   
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout: " + ex.Message + "\n" + ex.InnerException?.Message);
                return false;
            }
        }

        public User Login(string email, string password)
        {
            try
            {
                using (var db = GetDb())
                {
                    var user = db.Users.FirstOrDefault(u => u.Email == email);
                    if (user == null) return null;
                    if (!BC.Verify(password, user.PasswordHash)) return null;
                    return user;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login fout: " + ex.Message);
                return null;
            }
        }


        public bool ChangePassword(int userId, string newPassword)
        {
            using (var db = GetDb())
            {
                var user = db.Users.Find(userId);
                if (user == null) return false;
                user.PasswordHash = BC.HashPassword(newPassword);
                db.SaveChanges();
                return true;
            }
        }

        public bool DeleteAccount(int userId)
        {
            using (var db = GetDb())
            {
                var user = db.Users.Find(userId);
                if (user == null) return false;

                var posts = db.Posts.Where(p => p.UserId == userId).ToList();
                db.Posts.RemoveRange(posts);

                var items = db.ClothingItems.Where(i => i.UserId == userId).ToList();
                db.ClothingItems.RemoveRange(items);

                var collections = db.Collections.Where(c => c.UserId == userId).ToList();
                db.Collections.RemoveRange(collections);

                var follows = db.Follows.Where(f => f.FollowerId == userId ||
                                                    f.FollowingId == userId).ToList();
                db.Follows.RemoveRange(follows);

                var reports = db.Reports.Where(r => r.ReporterId == userId).ToList();
                db.Reports.RemoveRange(reports);

                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
        }

        public bool UpdateProfile(int userId, string bio, string picturePath, string username)
        {
            using (var db = GetDb())
            {
                var user = db.Users.Find(userId);
                if (user == null) return false;
                if (bio != null) user.Bio = bio;
                if (picturePath != null) user.ProfilePicturePath = picturePath;
                if (username != null) user.Username = username;
                db.SaveChanges();
                return true;
            }
        }
    }
}