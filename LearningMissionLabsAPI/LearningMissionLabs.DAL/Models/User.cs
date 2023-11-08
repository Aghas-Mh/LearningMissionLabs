namespace LearningMissionLabs.DAL.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Role { get; set; } = null!;
        public byte[] Salt { get; set; } = null!;
        public byte[] Hash { get; set; } = null!;
        public string? Token { get; set; } = null!;
    }
}