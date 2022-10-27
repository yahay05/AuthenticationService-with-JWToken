using System;
using Microsoft.AspNetCore.Identity;

namespace AandAService.Dal.Entities
{
    public class MyUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
