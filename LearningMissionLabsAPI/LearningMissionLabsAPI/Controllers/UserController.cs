using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LearningMissionLabs.BLL.Interfaces;
using LearningMissionLabs.DAL.Models;
using LearningMissionLabs.BLL.Models;
using LearningMissionLabsAPI.Requests;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearningMissionLabsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ISecurityService _securityService;

        public UserController(IUserService userService, ISecurityService securityService)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MessageRequest, MessageModel>();
                cfg.CreateMap<MessageModel, MessageResponse>();
                cfg.CreateMap<CreateGroupRequest, CreateGroupModel>();
            });
            _mapper = config.CreateMapper();

            _userService = userService;
            _securityService = securityService;
        }

        [HttpGet]
        public async Task<ActionResult<User>> Get()
        {
            return Ok(await _userService.GetMe());
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<ChatUserModel>>> GetAllUsers()
        {
            return await _userService.getAllUsers();
        }

        private async Task<ClientInfo> getClientInfo()
        {
            string clientId = Request.HttpContext.Connection.Id;
            var clientIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            int clientPort = Request.HttpContext.Connection.RemotePort;
            ClientInfo clientInfo = await _securityService.getClient(clientId, clientIpAddress.ToString(), clientPort);
            return clientInfo;
        }

        [HttpGet("MyMessages")]
        public async Task<ActionResult<List<MessageResponse>>> MyMessages()
        {
            ClientInfo clientInfo = await getClientInfo();
            var myMessageModels = await _userService.GetMyMessages(clientInfo);
            if (myMessageModels == null)
            {
                return Ok("No Messages");
            }
            List<MessageResponse> messagesResponse = new List<MessageResponse>();
            foreach (var messageModel in myMessageModels)
            {
                var message = _mapper.Map<MessageModel, MessageResponse>(messageModel);
                messagesResponse.Add(message);
            }
            return Ok(messagesResponse);
        }

        [HttpPost("Message")]
        public async Task<ActionResult<string>> Message(MessageRequest request)
        {
            var sendModel = _mapper.Map<MessageRequest, MessageModel>(request);
            bool isSaved = await _userService.SaveMessage(sendModel);
            if (!isSaved)
            {
                return "User not found";
            }
            return "Success";
        }

        [HttpPost("CreateGroup")]
        public async Task<ActionResult<string>> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var createGroupModel = _mapper.Map<CreateGroupRequest, CreateGroupModel>(request);
            var result = await _userService.CreateGroup(createGroupModel);
            if (result.Key == 200)
            {
                return Ok("Group Created");
            }
            return StatusCode(result.Key, result.Value);
        }
    }
}
