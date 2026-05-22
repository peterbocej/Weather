using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WeatherAPI.Application.DTO;
using WeatherAPI.Domain;
using WeatherAPI.Domain.Models.Security;
using WeatherAPI.Infrastructure.Repository;

namespace WebApi8.Services
{
   public interface IUserService
   {
      Task RegisterUser(RegisterUser registerUser);
      Task<bool> ValidateRegisterUser(RegisterUser registerUser);
      Task<bool> ValidateUser(string username, string password);
      Task<string> Login(Login login);
      Task<User> GetUserByUsername(string username);
   }
   public class UserService : IUserService
   {
      private readonly IUsersRepository _usersRepository;
      private readonly AppSettings _appSettings;
      private const int SaltSize = 16;
      private const int KeySize = 32;
      private const int Iterations = 100_000;

      public UserService(IUsersRepository usersRepository, AppSettings security)
      {
         _usersRepository = usersRepository;
         _appSettings = security;
      }
      #region Register
      public async Task RegisterUser(RegisterUser registerUser)
      {
         if (await ValidateRegisterUser(registerUser))
         {
            var user = await _usersRepository.CreateAsync(new User()
            {
               UserName = registerUser.UserName,
               UserEmail = registerUser.UserEmail,
               Password = HashPassword(registerUser.Password),
               Role = registerUser.Role
            });
            await _usersRepository.SaveAsync(null);
         }
         else
         {
            throw new Exception("Invalid registration details");
         }
      }

      public async Task<bool> ValidateRegisterUser(RegisterUser registerUser)
      {
         if (string.IsNullOrWhiteSpace(registerUser.UserName) || string.IsNullOrWhiteSpace(registerUser.Password) || string.IsNullOrWhiteSpace(registerUser.PasswordConfirmation))
         {
            throw new ArgumentNullException("User name, password, and password confirmation are required");
         }
         if (registerUser.Password != registerUser.PasswordConfirmation)
         {
            throw new ArgumentException("Passwords do not match");
         }
         if (await _usersRepository.ExistsAsync(u => u.UserName == registerUser.UserName))
         {
            throw new InvalidOperationException("Username already exists");
         }
         if (await _usersRepository.ExistsAsync(u => u.UserEmail == registerUser.UserEmail))
         {
            throw new InvalidOperationException("Email already exists");
         }

         return true;
      }
      #endregion

      public async Task<bool> ValidateUser(string username, string password)
      {
         if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
         {
            throw new ArgumentNullException("User name and password are required");
         }

         var user = await _usersRepository.GetByUsernameAsync(username);
         if (user == null)
            return false;

         return VerifyPassword(user.Password, password);
      }

      private string HashPassword(string password)
      {
         using var rng = RandomNumberGenerator.Create();
         var salt = new byte[SaltSize];
         rng.GetBytes(salt);

         using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
         var key = deriveBytes.GetBytes(KeySize);

         // format: {iterations}.{saltBase64}.{keyBase64}
         return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
      }

      private bool VerifyPassword(string storedHash, string password)
      {
         if (string.IsNullOrWhiteSpace(storedHash))
            return false;

         var parts = storedHash.Split('.', 3);
         if (parts.Length != 3)
            return false;

         if (!int.TryParse(parts[0], out var iterations))
            return false;

         var salt = Convert.FromBase64String(parts[1]);
         var key = Convert.FromBase64String(parts[2]);

         using var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
         var attemptedKey = deriveBytes.GetBytes(key.Length);

         return CryptographicOperations.FixedTimeEquals(attemptedKey, key);
      }

      public async Task<string> Login(Login login)
      {
         if (login == null) throw new ArgumentNullException(nameof(login));
         if (string.IsNullOrWhiteSpace(login.UserName) || string.IsNullOrWhiteSpace(login.Password))
            throw new ArgumentNullException("User name and password are required");

         var user = await _usersRepository.GetByUsernameAsync(login.UserName);
         if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

         if (!VerifyPassword(user.Password, login.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

         var claims = new[]
         {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.Email, user.UserEmail)
         };

         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Key));
         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

         var token = new JwtSecurityToken(
            issuer: _appSettings.Jwt.Issuer,
            audience: _appSettings.Jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_appSettings.Jwt.DurationInMinutes),
            signingCredentials: creds
         );

         return new JwtSecurityTokenHandler().WriteToken(token);
      }

      public async Task<User> GetUserByUsername(string username)
      {
         if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentNullException("User name is required");
         var user = await _usersRepository.GetByUsernameAsync(username);
         if (user == null)
            throw new KeyNotFoundException("User not found");
         return user;
      }
   }
}
