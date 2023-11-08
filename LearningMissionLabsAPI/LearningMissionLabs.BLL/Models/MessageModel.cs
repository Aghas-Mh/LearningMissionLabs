namespace LearningMissionLabs.BLL.Models
{
    public class MessageModel
    {
        public int id { get; set; }
        public string sender { get; set; } = null!;
        public string reciver { get; set; } = null!;
        public string message { get; set; } = null!;
    }
}
