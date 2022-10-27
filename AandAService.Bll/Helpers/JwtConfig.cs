using System;
using Microsoft.Extensions.Configuration;

namespace AandAService.Bll.Helpers
{
    public class JwtConfig
    {
        public string SecretKey { get; set; }

        public JwtConfig(IConfiguration configuration)
        {
            SecretKey = configuration["JwtConfig:SecretKey"];
        }
    }
}
