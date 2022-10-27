using System;
using System.ComponentModel.DataAnnotations;

namespace AandAService.Bll.Models.RequestModels
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
