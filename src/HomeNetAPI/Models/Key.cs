using System.ComponentModel.DataAnnotations;

namespace HomeNetAPI.Models
{
    public class Key
    {
        [Key, Required]
        public int KeyID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public int IsDeleted { get; set; }
    }
}
