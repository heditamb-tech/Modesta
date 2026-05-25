using Microsoft.EntityFrameworkCore;
using Modesta.Models;
using System.Linq;

namespace Modesta.Services
{
    public class FollowService
    {
        private static string _connString = @"Data Source=C:\Users\Heidi\modesta.db";

        private Modesta.Data.ModestDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<Modesta.Data.ModestDbContext>()
                .UseSqlite(_connString)
                .Options;
            return new Modesta.Data.ModestDbContext(options);
        }

        public void Follow(int followerId, int followingId, string targetPrivacy)
        {
            using (var db = GetDb())
            {
                if (db.Follows.Any(f => f.FollowerId == followerId &&
                                        f.FollowingId == followingId)) return;
                var status = targetPrivacy == "friends" ? "pending" : "accepted";
                db.Follows.Add(new Follow
                {
                    FollowerId = followerId,
                    FollowingId = followingId,
                    Status = status,
                    CreatedAt = System.DateTime.Now
                });
                db.SaveChanges();
            }
        }

        public void Unfollow(int followerId, int followingId)
        {
            using (var db = GetDb())
            {
                var f = db.Follows.FirstOrDefault(x =>
                    x.FollowerId == followerId &&
                    x.FollowingId == followingId);
                if (f != null) { db.Follows.Remove(f); db.SaveChanges(); }
            }
        }

        public bool IsFollowing(int followerId, int followingId)
        {
            using (var db = GetDb())
                return db.Follows.Any(f =>
                    f.FollowerId == followerId &&
                    f.FollowingId == followingId &&
                    f.Status == "accepted");
        }

        public int GetFollowerCount(int userId)
        {
            using (var db = GetDb())
                return db.Follows.Count(f =>
                    f.FollowingId == userId && f.Status == "accepted");
        }

        public int GetFollowingCount(int userId)
        {
            using (var db = GetDb())
                return db.Follows.Count(f =>
                    f.FollowerId == userId && f.Status == "accepted");
        }
    }
}