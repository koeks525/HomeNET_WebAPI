using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class DialingCode
    {
        [Required, Key]
        public int DialingCodeID { get; set; }
        [ForeignKey("CountryID")]
        public int CountryID { get; set; }
        [Required]
        public String Code { get; set; }
        [Required]
        public int IsDeleted { get; set; } = 0;

        public Country Country { get; set; }
    }
}
