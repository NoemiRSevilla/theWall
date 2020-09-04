using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace theWall.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int MessageId { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Message Blog { get; set; }
        public User Creator { get; set; }
    }
}