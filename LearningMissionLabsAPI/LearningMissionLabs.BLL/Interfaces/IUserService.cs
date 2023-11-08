using LearningMissionLabs.DAL.Models;
using LearningMissionLabs.BLL.Models;

namespace LearningMissionLabs.BLL.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetMe();
        public Task<List<ChatUserModel>> getAllUsers();
        public Task<bool> SaveMessage(MessageModel model); 
        public Task<List<MessageModel>> GetMyMessages(ClientInfo clientInfo);
        public Task<KeyValuePair<int, string>> CreateGroup(CreateGroupModel model);
    }
}
