using System.ComponentModel.DataAnnotations;

namespace Employer.Models.Domain
{
    public class Department
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(3, ErrorMessage = nameof(Cipher) + " length can't be more than 3.")]
        public string Cipher { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = nameof(Name) + "length can't be more than 100.")]
        public string Name { get; set; }
    }
}
