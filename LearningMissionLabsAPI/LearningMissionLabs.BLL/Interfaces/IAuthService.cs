using LearningMissionLabs.BLL.Models;
using LearningMissionLabs.BLL.Services;

namespace LearningMissionLabs.BLL.Interfaces
{
    public interface IAuthService
    {
        public Task<KeyValuePair<int, string>> Registration(UserModel userModel);
        public Task<KeyValuePair<int, string>> Login(UserModel userModel);
    }
}
