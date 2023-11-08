using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using LearningMissionLabs.BLL.Interfaces;
using LearningMissionLabs.BLL.Models;
using LearningMissionLabsAPI.Requests;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearningMissionLabsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _service;
        private readonly ISecurityService _securityService;

        public AuthController(ILogger<AuthController> logger, IAuthService AuthService, ISecurityService securityService) 
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LoginRequest, UserModel>();
                cfg.CreateMap<RegistrationRequest, UserModel>();
            });
            _mapper = config.CreateMapper();
            _logger = logger;
            _service = AuthService;
            _securityService = securityService;
        }

        public class Key
        {
            public string publicKey { get; set; } = null!;
        }

        [HttpPost("Connect")]
        public async Task<bool> Connect(Key key)
        {
            var clientId = Request.HttpContext.Connection.Id;
            var clientIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            var clientPort = Request.HttpContext.Connection.RemotePort;
            Console.WriteLine($"ID: {clientId}, IP: {clientIpAddress}, Port: {clientPort}");
            await _securityService.SetConnection(clientId, clientIpAddress.ToString(), clientPort, key.publicKey);
            return true;
        }

        [HttpGet("PublicKey")]
        public async Task<ActionResult<string>> PublicKey()
        
        {
            return await _securityService.GetPublicKey();
        }

        // POST api/<AuthController>
        [HttpPost("Registration")]
        public async Task<ActionResult<string>> Registration(RegistrationRequest request)
        {
            var userModel = _mapper.Map<RegistrationRequest, UserModel>(request);
            var result = await _service.Registration(userModel);
            if (result.Key == 200)
            {
                return Ok(result.Value);
            }
            return StatusCode(result.Key, result.Value);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginRequest loginRequest)
        {
            var userModel = _mapper.Map<LoginRequest, UserModel>(loginRequest);
            var result = await _service.Login(userModel);
            if (result.Key == 200)
            {
                return Ok(result.Value);
            }
            return StatusCode(result.Key, result.Value);
        }

        [HttpGet("Encrypt")]
        public async Task<ActionResult<string>> Encrypt(string message)
        {
            return await _securityService.Encrypt(message);
        }

        [HttpPost("Decrypt")]
        public async Task<ActionResult<string>> Decrypt(string message)
        {
            return await _securityService.Decrypt(message);
        }
    }
}
