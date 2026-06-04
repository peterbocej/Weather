using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Weather.Application.Services;
using Weather.Domain;
using Weather.Infrastructure.Data;
using Weather.Infrastructure.ExternalApi;
using Weather.Infrastructure.Repository;
using WebApi8.Services;

internal class Program
{
   private static void Main(string[] args)
   {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.

      builder.Services.AddControllers();
      // configure strongly typed settings objects
      builder.Services.Configure<IConfiguration>(builder.Configuration);
      var appSettings = new AppSettings();
      builder.Configuration.GetSection("AppSettings").Bind(appSettings);
      builder.Services.AddSingleton(appSettings);
      // logging
      builder.Services.AddLogging(o => o.AddConsole());
      // database contexts
      builder.Services.AddDbContext<SecurityDbContext>(options =>
         options.UseSqlite(builder.Configuration.GetConnectionString("Security")), ServiceLifetime.Scoped);
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
      // weather repositories and services
      builder.Services.AddScoped<ITemperatureResultRepository, TemperatureResultRepository>();
      builder.Services.AddScoped<IExternalWeatherApi, ExternalWeatherApi>();
      builder.Services.AddScoped<IWeatherService, WeatherService>();
      // authentication and swagger
      builder.Services.AddScoped<IUsersRepository, UsersRepository>();
      builder.Services.AddScoped<IUserService, UserService>();
      SetupSecurity(builder, appSettings);

      builder.WebHost.ConfigureKestrel(options =>
      {
         options.ListenAnyIP(8080);
         options.ListenAnyIP(8081, listenOptions =>
         {
            listenOptions.UseHttps("weatherapi.pfx", "asdfPwd");
         });
      });

      builder.Services.AddCors(options =>
      {
         options.AddDefaultPolicy(builder =>
         {
            builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
         });
      });

      var app = builder.Build();

      using (var scope = app.Services.CreateScope())
      {
         var cashContext = scope.ServiceProvider.GetRequiredService<CacheDbContext>();
         cashContext.Database.EnsureCreated();
         var securityContext = scope.ServiceProvider.GetRequiredService<SecurityDbContext>();
         securityContext.Database.EnsureCreated();
      }

      app.UseCors(conf =>
      {
         conf.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
      });
      app.UseSwagger();
      app.UseSwaggerUI();

      app.UseHttpsRedirection();

      app.UseAuthentication();
      app.UseAuthorization();

      app.MapControllers();

      app.Run();
   }

   private static void SetupSecurity(WebApplicationBuilder builder, AppSettings appSettings)
   {
      // Add authentication services
      builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
         .AddJwtBearer(options =>
         {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
               ValidateIssuer = true,
               ValidIssuer = appSettings.Jwt.Issuer,
               ValidateAudience = true,
               ValidAudience = appSettings.Jwt.Audience,
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Key)),
               ValidateLifetime = true
            };
            options.Events = new JwtBearerEvents
            {
               OnAuthenticationFailed = context =>
               {
                  ILogger logger = LoggerFactory.Create(config => config.AddConsole())
                     .CreateLogger("Program");
                  logger.LogError(context.Exception, "Authentication failed.");
                  return Task.CompletedTask;
               }
            };
         });


      builder.Services.AddSwaggerGen(options =>
      {
         // Add JWT bearer definition to Swagger
         options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
         {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer {token}'"
         });

         options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
               new string[] { }
            }
         });
      });
   }
}