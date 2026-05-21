using System;
using System.ComponentModel.DataAnnotations;

namespace Modesta.Models
{
    public class Follow
    {
        [Key]
        public int FollowId { get; set; }
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }
        public string Status { get; set; } = "accepted";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}