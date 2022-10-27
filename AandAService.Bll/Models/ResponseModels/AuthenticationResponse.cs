using System;
namespace AandAService.Bll.Models.ResponseModels
{
    public class AuthenticationResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string JwtToken { get; set; }
    }
}
