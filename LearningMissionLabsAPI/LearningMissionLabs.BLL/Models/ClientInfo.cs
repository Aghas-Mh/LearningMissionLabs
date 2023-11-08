namespace LearningMissionLabs.BLL.Models
{
    public class ClientInfo
    {
        public string Id { get; set; } = null!;
        public string IP { get; set; } = null!;
        public int Port { get; set; }
        public string publicKey { get; set; } = null!;
    }
}
