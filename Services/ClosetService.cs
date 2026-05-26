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
                var all = db.Collections
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Items)
                    .ToList();

                // Sorteer: Opgeslagen items, Mijn items, Outfit builder, dan de rest
                var ordered = new List<Collection>();
                var saved = all.FirstOrDefault(c => c.Name == "Opgeslagen items");
                var mine = all.FirstOrDefault(c => c.Name == "Mijn items");
                var builder = all.FirstOrDefault(c => c.Name == "Outfit builder");

                if (saved != null) ordered.Add(saved);
                if (mine != null) ordered.Add(mine);
                if (builder != null) ordered.Add(builder);

                foreach (var c in all)
                    if (c.Name != "Opgeslagen items" &&
                        c.Name != "Mijn items" &&
                        c.Name != "Outfit builder")
                        ordered.Add(c);

                return ordered;
            }
        }

        public Collection CreateCollection(int userId, string name)
        {
            using (var db = GetDb())
            {
                // Geen dubbele collecties aanmaken
                var existing = db.Collections.FirstOrDefault(
                    c => c.UserId == userId && c.Name == name);
                if (existing != null) return existing;

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

        public Collection GetOrCreateMyItemsCollection(int userId)
        {
            using (var db = GetDb())
            {
                var col = db.Collections.FirstOrDefault(
                    c => c.UserId == userId && c.Name == "Mijn items");
                if (col == null)
                {
                    col = new Collection
                    {
                        UserId = userId,
                        Name = "Mijn items",
                        CreatedAt = DateTime.Now
                    };
                    db.Collections.Add(col);
                    db.SaveChanges();
                }
                return col;
            }
        }

        public Collection GetOrCreateOutfitBuilderCollection(int userId)
        {
            using (var db = GetDb())
            {
                var col = db.Collections.FirstOrDefault(
                    c => c.UserId == userId && c.Name == "Outfit builder");
                if (col == null)
                {
                    col = new Collection
                    {
                        UserId = userId,
                        Name = "Outfit builder",
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
                    Price = price,
                    CreatedAt = DateTime.Now
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

        public void SaveOutfit(int userId, string name, List<int> itemIds)
        {
            using (var db = GetDb())
            {
                // Zorg dat Outfit builder collectie bestaat
                GetOrCreateOutfitBuilderCollection(userId);

                // Maak nieuwe collectie aan met de naam van de outfit
                var outfitCol = new Collection
                {
                    UserId = userId,
                    Name = name,
                    CreatedAt = DateTime.Now
                };
                db.Collections.Add(outfitCol);
                db.SaveChanges();

                foreach (var itemId in itemIds)
                {
                    var item = db.ClothingItems.Find(itemId);
                    if (item != null)
                    {
                        var copy = new ClothingItem
                        {
                            UserId = userId,
                            CollectionId = outfitCol.CollectionId,
                            Name = item.Name,
                            Category = item.Category,
                            Color = item.Color,
                            Brand = item.Brand,
                            PhotoPath = item.PhotoPath,
                            Price = item.Price,
                            CreatedAt = DateTime.Now
                        };
                        db.ClothingItems.Add(copy);
                    }
                }
                db.SaveChanges();
                System.Windows.MessageBox.Show(
                    $"Outfit '{name}' opgeslagen!", "Gelukt");
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

        public void UpdateItemPhoto(int itemId, string photoPath)
        {
            using (var db = GetDb())
            {
                var item = db.ClothingItems.Find(itemId);
                if (item != null)
                {
                    item.PhotoPath = photoPath;
                    db.SaveChanges();
                }
            }
        }

        public void AddItemToCollection(int itemId, int collectionId)
        {
            using (var db = GetDb())
            {
                var item = db.ClothingItems.Find(itemId);
                if (item == null) return;

                bool exists = db.ClothingItems.Any(i =>
                    i.CollectionId == collectionId &&
                    i.Name == item.Name &&
                    i.UserId == item.UserId);

                if (exists)
                {
                    System.Windows.MessageBox.Show(
                        "Dit item staat al in deze collectie!", "Al aanwezig");
                    return;
                }

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
                    CreatedAt = DateTime.Now
                };
                db.ClothingItems.Add(newItem);
                db.SaveChanges();
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