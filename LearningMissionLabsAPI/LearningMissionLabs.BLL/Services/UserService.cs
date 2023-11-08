using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LearningMissionLabs.BLL.Interfaces;
using LearningMissionLabs.DAL;
using System.Security.Claims;
using LearningMissionLabs.DAL.Models;
using LearningMissionLabs.BLL.Models;
using AutoMapper;

namespace LearningMissionLabs.BLL.Services
{
    public class UserService : IUserService
    {
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILearningMissionContext _dbContext;
        private readonly ISecurityService _secureService;

        public UserService(IHttpContextAccessor httpContextAccessor, ILearningMissionContext dbContext, ISecurityService secureService)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MessageModel, Message>();
                cfg.CreateMap<Message, MessageModel>();
            });
            _mapper = config.CreateMapper();
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _secureService = secureService;
        }

        public async Task<User> GetMe()
        {
            if (_httpContextAccessor == null)
            {
                return null;
            }
            string Email = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Email)!;
            User user = await _dbContext.Users.Where(user => user.Email == Email).FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<ChatUserModel>> getAllUsers()
        {
            string Email = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Email)!;
            var chatUsersModelList = new List<ChatUserModel>();
            var usersList = await _dbContext.Users.ToListAsync();
            foreach (var user in usersList)
            {
                if (user.Email == Email)
                {
                    continue;
                }
                var chatUserModel = new ChatUserModel
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.Name,
                };
                chatUsersModelList.Add(chatUserModel);
            }
            return chatUsersModelList;
        }

        public async Task<bool> SaveMessage(MessageModel model)
        {
            User reciver = _dbContext.Users.Where(user => user.Email == model.reciver).FirstOrDefault()!;
            if (reciver == null)
            {
                return false;
            }
            User sender = await GetMe();
            var message = _mapper.Map<MessageModel, Message>(model);
            message.senderID = sender.Id;
            message.reciverID = reciver.Id;
            _dbContext.Messages.Add(message);
            int result = await _dbContext.SaveChangesAsync();
            Console.WriteLine($"SAVE RESULT: {result}");
            return true;
        }

        public async Task<List<MessageModel>> GetMyMessages(ClientInfo clientInfo)
        {
            User user = await GetMe();
            var myMessages = _dbContext.Messages.Where(message => message.senderID == user.Id || message.reciverID == user.Id).ToList();
            string? userPublicKey = SecurityService._clientsInfo.Where(client => client.Id == clientInfo.Id).FirstOrDefault()?.publicKey;
            if (myMessages == null || myMessages.Count == 0 || userPublicKey == null)
            {
                return null;
            }
            List<MessageModel> messageModels = new List<MessageModel>();
            foreach (var message in myMessages)
            {
                var model = _mapper.Map<Message, MessageModel>(message);
                model.sender = _dbContext.Users.Where(user => user.Id == message.senderID).FirstOrDefault().Email;
                model.reciver = _dbContext.Users.Where(user => user.Id == message.reciverID).FirstOrDefault().Email;
                string decrypted = await _secureService.Decrypt(model.message);
                string encrypted = await _secureService.Encrypt(decrypted, userPublicKey);
                model.message = encrypted;
                messageModels.Add(model);
            }
            return messageModels;
        }

        public async Task<KeyValuePair<int, string>> CreateGroup(CreateGroupModel model)
        {
            User creator = await GetMe();
            Group group = new Group();
            group.creatorID = creator.Id;
            group.groupName = model.groupName;
            List<User> usersList = new List<User> { creator };
            foreach (string addingUserEmail in model.users)
            {
                var findedUser = _dbContext.Users.Where(user => user.Email == addingUserEmail).FirstOrDefault();
                if (findedUser == null)
                {
                    return new KeyValuePair<int, string>(650, $"User {addingUserEmail} not found");
                }
                usersList.Add(findedUser);
            }
            var changes = _dbContext.Groups.Add(group);
            var result = await _dbContext.SaveChangesAsync();
            Group addedGroup = changes.Entity;
            Console.WriteLine($"Added group ID: ", addedGroup.groupID);
            foreach (User user in usersList)
            {
                _dbContext.usersInGroup.Add(new UserInGroup
                {
                    groupID = addedGroup.groupID,
                    userID = user.Id
                });
            }
            await _dbContext.SaveChangesAsync();

            return new KeyValuePair<int, string>(200, "Group created");
        }
    }
}
