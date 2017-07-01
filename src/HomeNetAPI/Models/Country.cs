using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HomeNetAPI.Models
{
    public class Country
    {
        [Key,Required]
        public int CountryID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int IsDeleted { get; set; }

        public ICollection<DialingCode> DialingCodes { get; set; }
    }
}
