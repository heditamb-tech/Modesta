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

        public Collection GetOrCreateSavedCollection(int userId)
        {
            using (var db = GetDb())
            {
                var col = db.Collections.FirstOrDefault(
                    c => c.UserId == userId && c.Name == "Opgeslagen items");
                if (col == null)
                {
                    col = new Collection
                    {
                        UserId = userId,
                        Name = "Opgeslagen items",
                        CreatedAt = DateTime.Now
                    };
                    db.Collections.Add(col);
                    db.SaveChanges();
                }
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

        public void UpdateItem(int itemId, string name, string category)
        {
            using (var db = GetDb())
            {
                var item = db.ClothingItems.Find(itemId);
                if (item != null)
                {
                    item.Name = name;
                    item.Category = category;
                    db.SaveChanges();
                }
            }
        }

        public void AddItemToCollection(int itemId, int collectionId)
        {
            using (var db = GetDb())
            {
                var item = db.ClothingItems.Find(itemId);
                if (item != null)
                {
                    // Maak een kopie van het item in de nieuwe collectie
                    var newItem = new ClothingItem
                    {
                        UserId = item.UserId,
                        CollectionId = collectionId,
                        Name = item.Name,
                        Category = item.Category,
                        Color = item.Color,
                        Brand = item.Brand,
                        PhotoPath = item.PhotoPath,
                        Price = item.Price,
                        TimesWorn = 0,
                        CreatedAt = DateTime.Now
                    };
                    db.ClothingItems.Add(newItem);
                    db.SaveChanges();
                }
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