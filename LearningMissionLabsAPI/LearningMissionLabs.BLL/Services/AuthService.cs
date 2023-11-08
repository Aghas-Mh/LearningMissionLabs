using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using LearningMissionLabs.BLL.Interfaces;
using LearningMissionLabs.BLL.Models;
using LearningMissionLabs.DAL;
using LearningMissionLabs.DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LearningMissionLabs.BLL.Services
{
    public class AuthService : IAuthService
    {
        private IMapper _mapper;
        private readonly ILearningMissionContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;

        public enum RoleType
        {
            User,
            Admin,
            Chief
        }

        public AuthService(ILearningMissionContext dbContext, IConfiguration configuration, ISecurityService securityService)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserModel, User>();
            });
            _mapper = config.CreateMapper();
            _dbContext = dbContext;
            _configuration = configuration;
            _securityService = securityService;
        }

        public async Task<KeyValuePair<int, string>> Registration(UserModel userModel)
        {
            userModel.Email = await _securityService.Decrypt(userModel.Email);
            userModel.Name = await _securityService.Decrypt(userModel.Name);
            userModel.Password = await _securityService.Decrypt(userModel.Password);
            userModel.Confirm = await _securityService.Decrypt(userModel.Confirm);
            if (userModel.Password != userModel.Confirm)
            {
                return new KeyValuePair<int, string>(612, "Confirm does not match password");
            }
            CreatePasswordHash(userModel.Password, out byte[] passwordHash, out byte[] passwordSalt);
            userModel.Role = RoleType.User.GetDisplayName();
            userModel.Hash = passwordHash;
            userModel.Salt = passwordSalt;
            var user = _mapper.Map<UserModel, User>(userModel);
            _dbContext.Users.Add(user);
            int result = 0;
            try
            {
                result = await _dbContext.SaveChangesAsync();
            } catch (Exception ex)
            {
                if (ex.InnerException?.GetType()?.Name == "SqlException")
                {
                    return new KeyValuePair<int, string>(610, "This Email is already registered");
                }
                return new KeyValuePair<int, string>(400, "Registration failed. Unhandled Exeption");
            }
            if (result == 1)
            {
                return new KeyValuePair<int, string>(200, "Registration was successful!");
            }
            return new KeyValuePair<int, string>(611, "Registration failed. Unprocessed!");
        }

        public async Task<KeyValuePair<int, string>> Login(UserModel userModel)
        {
            userModel.Email = await _securityService.Decrypt(userModel.Email);
            userModel.Password = await _securityService.Decrypt(userModel.Password);
            User user = user = _dbContext.Users.Where(user => user.Email == userModel.Email).FirstOrDefault()!;
            if (user == null)
            {
                return new KeyValuePair<int, string>(620, "Email not registered");
            }
            if (!VerifyPasswordHash(userModel.Password, user.Hash, user.Salt))
            {
                return new KeyValuePair<int, string>(621, "Wrong Password");
            }
            user.Token = CreateToken(user);
            await _dbContext.SaveChangesAsync();
            return new KeyValuePair<int, string>(200, user.Token);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF32.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF32.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF32.GetBytes(
                _configuration.GetSection("AppSettings:SecretKey").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
