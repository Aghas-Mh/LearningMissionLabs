namespace LearningMissionLabs.DAL.Models
{
    public class ChatUserModel
    {
        public int id { get; set; }
        public string email { get; set; } = null!;
        public string name { get; set; } = null!;
        public bool status { get; set; }
    }
}
