using DDDProject.Domain.Models;
using DDDProject.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// JWT认证服务扩展
    /// </summary>
    public static class JwtAuthenticationExtension
    {
        /// <summary>
        /// 添加JWT认证服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddAuthentication(authen =>
            {
                authen.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authen.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false; // 在开发环境中可以设为false
                bearer.SaveToken = true;
                
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddScoped<CurrentUser>();

            return services;
        }
    }
}