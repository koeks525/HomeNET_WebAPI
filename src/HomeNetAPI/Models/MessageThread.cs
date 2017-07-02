﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class MessageThread
    {
        [Key,Required]
        public int MessageThreadID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int Priority { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        public HouseMember HouseMember { get; set; }

    }
}