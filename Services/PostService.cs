using Microsoft.EntityFrameworkCore;
using Modesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modesta.Services
{
    public class PostService
    {
        private static string _connString = @"Data Source=C:\Users\Heidi\modesta.db";

        private Modesta.Data.ModestDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<Modesta.Data.ModestDbContext>()
                .UseSqlite(_connString)
                .Options;
            return new Modesta.Data.ModestDbContext(options);
        }

        public Post CreatePost(int userId, string imagePath, string caption, string privacy)
        {
            using (var db = GetDb())
            {
                var post = new Post
                {
                    UserId = userId,
                    ImageUrl = imagePath,
                    Caption = caption,
                    PrivacySetting = privacy,
                    CreatedAt = DateTime.Now
                };
                db.Posts.Add(post);
                db.SaveChanges();
                return post;
            }
        }

        public void AddTag(int postId, string itemType, string brand,
                           string onlineLink, string shopName, string shopAddress)
        {
            using (var db = GetDb())
            {
                var tag = new ItemTag
                {
                    PostId = postId,
                    ItemType = itemType,
                    Brand = brand,
                    OnlineLink = onlineLink,
                    ShopName = shopName,
                    ShopAddress = shopAddress
                };
                db.ItemTags.Add(tag);
                db.SaveChanges();
            }
        }

        public List<Post> GetFeed(int userId)
        {
            using (var db = GetDb())
            {
                return db.Posts
                    .Include(p => p.User)
                    .Include(p => p.ItemTags)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();
            }
        }

        public List<Post> Search(string query)
        {
            using (var db = GetDb())
            {
                return db.Posts
                    .Where(p => p.Caption.Contains(query) ||
                        p.ItemTags.Any(t => t.Brand.Contains(query) ||
                                            t.ItemType.Contains(query)) ||
                        p.User.Username.Contains(query))
                    .Include(p => p.User)
                    .Include(p => p.ItemTags)
                    .ToList();
            }
        }

        public List<Post> GetUserPosts(int userId)
        {
            using (var db = GetDb())
            {
                return db.Posts
                    .Where(p => p.UserId == userId)
                    .Include(p => p.ItemTags)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();
            }
        }

        public void ReportPost(int postId, int reporterId, string reason)
        {
            using (var db = GetDb())
            {
                var report = new Report
                {
                    PostId = postId,
                    ReporterId = reporterId,
                    Reason = reason,
                    Status = "pending",
                    CreatedAt = DateTime.Now
                };
                db.Reports.Add(report);
                db.SaveChanges();
            }
        }
    }
}