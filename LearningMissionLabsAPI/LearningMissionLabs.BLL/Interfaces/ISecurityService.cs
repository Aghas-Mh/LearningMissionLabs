using LearningMissionLabs.BLL.Services;
using LearningMissionLabs.BLL.Models;


namespace LearningMissionLabs.BLL.Interfaces
{
    public interface ISecurityService
    {
        public Task SetConnection(string id, string ip, int port, string publickey);
        public Task<ClientInfo> getClient(string id, string ip, int port);
        public Task<string> getClientKey(string id, string ip, int port);
        public Task<string> GetPublicKey();
        public Task<string> Encrypt(string message);
        public Task<string> Encrypt(string message, string publicKey);
        public Task<string> Decrypt(string message);
    }
}
