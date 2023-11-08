using Microsoft.AspNetCore.Mvc;

namespace LearningMissionLabsAPI.Responses
{
    public class PostResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public FileContentResult ImageContent { get; set; } = null!;
    }
}
