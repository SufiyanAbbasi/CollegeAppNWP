using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CollegeApp.Models
{
    public class StudentDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student Name is required")]
        [StringLength(50)]
        public string StudentName { get; set; }

        [EmailAddress(ErrorMessage = "Enter Valid Email")]
        [Required]
        public string Email { get; set; }


        [Required]
        public string Address { get; set; }

        public DateTime DOB {  get; set; }

 


    }
}
