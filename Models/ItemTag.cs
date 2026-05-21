using System;
using System.ComponentModel.DataAnnotations;

namespace Modesta.Models
{
    public class ItemTag
    {
        [Key]
        public int TagId { get; set; }
        public int PostId { get; set; }
        public string ItemType { get; set; }
        public string Brand { get; set; }
        public string OnlineLink { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ProductPhotoPath { get; set; }
        public Post Post { get; set; }
    }
}