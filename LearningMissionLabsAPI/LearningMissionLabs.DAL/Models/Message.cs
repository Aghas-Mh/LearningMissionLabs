namespace LearningMissionLabs.DAL.Models
{
    public class Message
    {
        public int id { get; set; }
        public int senderID { get; set; }
        public int reciverID { get; set; }
        public string message { get; set; } = null!;
    }
}
