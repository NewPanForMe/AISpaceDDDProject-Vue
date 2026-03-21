using DDDProject.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            var jwtSettings = new JwtSettings();
            services.Configure<JwtSettings>(options =>
            {
                options.Issuer = jwtSettings.Issuer;
                options.Audience = jwtSettings.Audience;
                options.Key = jwtSettings.Key;
                options.ExpireMinutes = jwtSettings.ExpireMinutes;
            });

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ?? "YourSuperSecretKey12345678901234567890")),
                    ValidateIssuer = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "DDDProject",
                    ValidateAudience = true,
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "DDDProject",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddScoped<CurrentUser>();
            
            return services;
        }
    }
}