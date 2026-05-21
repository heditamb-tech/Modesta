using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Modesta.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Caption { get; set; }
        public string ImageUrl { get; set; }
        public string PrivacySetting { get; set; } = "public";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public User User { get; set; }
        public List<ItemTag> ItemTags { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<Report> Reports { get; set; } = new();
    }
}