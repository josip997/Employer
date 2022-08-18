using DataAnnotationsExtensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Employer.Models
{
    public class IndexEmployeeViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = nameof(Name) + " length can't be more than 100.")]
        public string Name { get; set; }

        [Required]
        [Email]
        public string Email { get; set; }

        [Required]
        [Range(0, long.MaxValue, ErrorMessage = nameof(Salary) + " exsceeds maximum value.")]
        public long Salary { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }

        public IEnumerable<SelectListItem>? Departments { get; set; }

        public string? DepartmentName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        
    }
}
