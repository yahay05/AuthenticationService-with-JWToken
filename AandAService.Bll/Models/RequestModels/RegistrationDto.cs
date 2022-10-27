using System;
using System.ComponentModel.DataAnnotations;

namespace AandAService.Bll.Models.RequestModels
{
    public class RegistrationDto
    {
        [Required]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        [DataType(DataType.Text)]
        [Required]
        public string LastName { get; set; }
        [DataType(DataType.Text)]
        [Required]
        public string UserName { get; set; }
        public int Age { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //Dto means data transfer object : "yahay05"
    }
}
