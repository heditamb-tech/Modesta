using Microsoft.EntityFrameworkCore;
using Modesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modesta.Services
{
    public class ClosetService
    {
        private static string _connString = @"Data Source=C:\Users\Heidi\modesta.db";

        private Modesta.Data.ModestDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<Modesta.Data.ModestDbContext>()
                .UseSqlite(_connString)
                .Options;
            return new Modesta.Data.ModestDbContext(options);
        }

        public List<Collection> GetCollections(int userId)
        {
            using (var db = GetDb())
            {
                return db.Collections
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Items)
                    .ToList();
            }
        }

        public Collection CreateCollection(int userId, string name)
        {
            using (var db = GetDb())
            {
                var col = new Collection
                {
                    UserId = userId,
                    Name = name,
                    CreatedAt = DateTime.Now
                };
                db.Collections.Add(col);
                db.SaveChanges();
                return col;
            }
        }

        public ClothingItem AddItem(int userId, int collectionId,
            string name, string category, string color,
            string brand, string photoPath, decimal price)
        {
            using (var db = GetDb())
            {
                var item = new ClothingItem
                {
                    UserId = userId,
                    CollectionId = collectionId,
                    Name = name,
                    Category = category,
                    Color = color,
                    Brand = brand,
                    PhotoPath = photoPath,
                    Price = price
                };
                db.ClothingItems.Add(item);
                db.SaveChanges();
                return item;
            }
        }

        public List<ClothingItem> GetItems(int userId)
        {
            using (var db = GetDb())
            {
                return db.ClothingItems
                    .Where(i => i.UserId == userId)
                    .Include(i => i.Collection)
                    .ToList();
            }
        }

        public void DeleteItem(int itemId)
        {
            using (var db = GetDb())
            {
                var item = db.ClothingItems.Find(itemId);
                if (item != null)
                {
                    db.ClothingItems.Remove(item);
                    db.SaveChanges();
                }
            }
        }
    }
}