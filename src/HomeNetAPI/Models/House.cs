﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace HomeNetAPI.Models
{
    public class House
    {
        [Key,Required]
        public int HouseID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }       
        //reference to the house profile picture
        public string HouseImage { get; set; }

        public string ResizedHouseImage { get; set; }
        
        public string Location { get; set; } //Store GPS data which will get resolved to a country name, city, etc. 
        
        public string DateCreated { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        [ForeignKey("Id")]
        public int OwnerID { get; set; }

        public User User { get; set; }

        public ICollection<HouseProfileImage> HouseProfileImages { get; set; }
    }
}
