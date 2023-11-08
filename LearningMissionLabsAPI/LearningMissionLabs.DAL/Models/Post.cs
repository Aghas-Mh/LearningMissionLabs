namespace LearningMissionLabs.DAL.Models
{
    public partial class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImagePath { get; set; }
    }
}
