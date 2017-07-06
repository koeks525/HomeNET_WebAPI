using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HomeNetAPI.Models
{
    public class User : IdentityUser<int>
    {
        [Key, Required]
        public override int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required, EmailAddress]
        public override string Email { get; set; }

        [Required]
        public string DateOfBirth { get; set; }

        [Required]
        public override string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public string SecurityQuestion { get; set; }
       
        public string SecurityAnswer { get; set; }

        [Required]
        public int IsDeleted { get; set; }

        [Required]
        public string DateRegistered { get; set; }

        [Required]
        public string Gender { get; set; }

        private int IsActive { get; set; } = 0;

        [ForeignKey("CountryID")]
        public int CountryID { get; set; }
        public Country Country { get; set; }
        public override string PhoneNumber { get; set; }
        public string SkypeID { get; set; }
        public string FacebookID { get; set; }
        public string TwitterID { get; set; }
        //Link to the users profile pictue
        public string ProfileImage { get; set; }
        public ICollection<House> Houses { get; set; }
        
        public string FirebaseMessagingToken { get; set; }
    }
}
