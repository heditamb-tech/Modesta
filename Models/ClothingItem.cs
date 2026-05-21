using System;
using System.ComponentModel.DataAnnotations;

namespace Modesta.Models
{
    public class ClothingItem
    {
        [Key]

        public int ItemId { get; set; }
        public int UserId { get; set; }
        public int CollectionId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string PhotoPath { get; set; }
        public int TimesWorn { get; set; } = 0;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Collection Collection { get; set; }
    }
}
