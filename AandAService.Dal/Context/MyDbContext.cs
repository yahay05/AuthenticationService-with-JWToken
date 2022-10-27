using System;
using AandAService.Dal.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AandAService.Dal.Context
{
    public class MyDbContext : IdentityDbContext<MyUser,MyRole,Guid>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

    }
}
