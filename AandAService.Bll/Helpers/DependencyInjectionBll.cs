using System;
using AandAService.Bll.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AandAService.Bll.Helpers
{
    public static class DependencyInjectionBll
    {
        public static void DependencyBll(this IServiceCollection services)
        {
            services.AddScoped<JwtConfig>();
            services.AddAutoMapper(typeof(MapperProfile));
            services.AddScoped<AccountService>();
        }
    } 
}
