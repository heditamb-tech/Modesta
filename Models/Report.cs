using System;
using System.ComponentModel.DataAnnotations;

namespace Modesta.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }
        public int PostId { get; set; }
        public int ReporterId { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Post Post { get; set; }
    }
}