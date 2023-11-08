using Microsoft.AspNetCore.Http;

namespace LearningMissionLabs.BLL.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IFormFile? Image { get; set; } = null!;
        public string? ImagePath { get; set; }
        public byte[] ImageBytes { get; set; }
    }
}
