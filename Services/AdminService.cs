using Microsoft.EntityFrameworkCore;
using Modesta.Models;
using System.Collections.Generic;
using System.Linq;

namespace Modesta.Services
{
    public class AdminService
    {
        private static string _connString = @"Data Source=C:\Users\Heidi\modesta.db";

        private Modesta.Data.ModestDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<Modesta.Data.ModestDbContext>()
                .UseSqlite(_connString)
                .Options;
            return new Modesta.Data.ModestDbContext(options);
        }

        public List<User> GetAllUsers()
        {
            using (var db = GetDb())
                return db.Users.ToList();
        }

        public void DeleteUser(int userId)
        {
            using (var db = GetDb())
            {
                var user = db.Users.Find(userId);
                if (user != null) { db.Users.Remove(user); db.SaveChanges(); }
            }
        }

        public List<Report> GetPendingReports()
        {
            using (var db = GetDb())
                return db.Reports
                    .Where(r => r.Status == "pending")
                    .Include(r => r.Post)
                    .ToList();
        }

        public void RemoveReportedPost(int reportId)
        {
            using (var db = GetDb())
            {
                var report = db.Reports.Include(r => r.Post)
                    .FirstOrDefault(r => r.ReportId == reportId);
                if (report != null)
                {
                    if (report.Post != null) db.Posts.Remove(report.Post);
                    report.Status = "removed";
                    db.SaveChanges();
                }
            }
        }

        public void IgnoreReport(int reportId)
        {
            using (var db = GetDb())
            {
                var report = db.Reports.Find(reportId);
                if (report != null) { report.Status = "ignored"; db.SaveChanges(); }
            }
        }

        public int GetUserCount()
        {
            using (var db = GetDb())
                return db.Users.Count();
        }

        public int GetPostCount()
        {
            using (var db = GetDb())
                return db.Posts.Count();
        }

        public int GetPendingReportCount()
        {
            using (var db = GetDb())
                return db.Reports.Count(r => r.Status == "pending");
        }
    }
}