using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using WeatherAPI.Application.Services;
using WeatherAPI.Domain;
using WeatherAPI.Infrastructure.Data;
using WeatherAPI.Infrastructure.ExternalApi;
using WeatherAPI.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<IConfiguration>(builder.Configuration);
var settings = new Settings(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<Settings>(settings);
builder.Services.AddLogging(o => o.AddConsole());
switch (settings.Cache.Mode)
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

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication("Bearer")
   .AddJwtBearer("Bearer", options =>
   {
      options.TokenValidationParameters = new TokenValidationParameters
      {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = settings.Jwt.Issuer,
         ValidAudience = settings.Jwt.Audience,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Jwt.Key))
      };
   });
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
   c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherAPI", Version = "v1" });
   c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
   {
      Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
      Name = "Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.ApiKey,
      Scheme = "Bearer",
      BearerFormat = "JWT"
   });
   c.AddSecurityRequirement(document => new()
   {
      [new OpenApiSecuritySchemeReference("Bearer", document)] = []
   });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
   var dbContext = scope.ServiceProvider.GetRequiredService<CacheDbContext>();
   dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.MapOpenApi();
   app.UseSwagger();
   app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
