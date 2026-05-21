using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Modesta.Models
{
    public class Collection
    {
        [Key] 
        public int CollectionId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<ClothingItem> Items { get; set; } = new();
    }
}
