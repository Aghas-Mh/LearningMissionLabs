namespace LearningMissionLabs.BLL.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Confirm {  get; set; } = null!;
        public byte[] Salt { get; set; } = null!;
        public byte[] Hash { get; set; } = null!;
    }
}
