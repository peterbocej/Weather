using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WeatherAPI.Application.Services;
using WeatherAPI.Domain;
using WeatherAPI.Infrastructure.Data;
using WeatherAPI.Infrastructure.ExternalApi;
using WeatherAPI.Infrastructure.Repository;

internal class Program
{
   private static void Main(string[] args)
   {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.

      builder.Services.AddControllers();
      builder.Services.Configure<IConfiguration>(builder.Configuration);
      var appSettings = new AppSettings();
      builder.Configuration.GetSection("AppSettings").Bind(appSettings);
      builder.Services.AddSingleton(appSettings);
      builder.Services.AddLogging(o => o.AddConsole());
      switch (appSettings.Cache.Mode)
      {
         case CacheMode.None:
         case CacheMode.Memory:
            builder.Services.AddDbContext<CacheDbContext>(options =>
               options.UseInMemoryDatabase("WeatherCache"), ServiceLifetime.Scoped);
            break;
         case CacheMode.Database:
            builder.Services.AddDbContext<CacheDbContext>(options =>
               options.UseSqlite(builder.Configuration.GetConnectionString("WeatherCache")), ServiceLifetime.Scoped);
            break;
      }
      builder.Services.AddScoped<ITemperatureResultRepository, TemperatureResultRepository>();
      builder.Services.AddScoped<IExternalWeatherApi, ExternalWeatherApi>();
      builder.Services.AddScoped<IWeatherService, WeatherService>();

      SetupSecurity(builder, appSettings);

      var app = builder.Build();

      using (var scope = app.Services.CreateScope())
      {
         var dbContext = scope.ServiceProvider.GetRequiredService<CacheDbContext>();
         dbContext.Database.EnsureCreated();
      }

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
         app.UseSwagger();
         app.UseSwaggerUI();
      }

      app.UseHttpsRedirection();

      app.UseAuthentication();
      app.UseAuthorization();

      app.MapControllers();

      app.Run();
   }

   private static void SetupSecurity(WebApplicationBuilder builder, AppSettings appSettings)
   {
      // Add authentication services
      builder.Services.AddAuthentication(options =>
      {
         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options =>
      {
         options.TokenValidationParameters = new TokenValidationParameters
         {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = appSettings.Jwt.Issuer,
            ValidAudience = appSettings.Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Key))
         };
         options.Events = new JwtBearerEvents
         {
            OnAuthenticationFailed = context =>
            {
               var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
               logger.LogError(context.Exception, "Authentication failed: {message}", context.Exception.Message);
               return Task.CompletedTask;
            }
         };
      });
      builder.Services.AddAuthorization();

      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen(c =>
      {
         c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherAPI", Version = "v1" });

         // Add JWT Bearer definition
         c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
         {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your valid token.\nExample: Bearer eyJhbGciOiJIUzI1NiIs...",
         });

         // Apply JWT Bearer globally
         c.AddSecurityRequirement(new OpenApiSecurityRequirement
         {
            {
               new OpenApiSecurityScheme
               {
                  Reference = new OpenApiReference
                  {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                  }
               },
               Array.Empty<string>()
            }
         });
      });
   }
}