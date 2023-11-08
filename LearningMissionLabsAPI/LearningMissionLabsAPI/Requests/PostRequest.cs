namespace LearningMissionLabsAPI.Requests
{
    public class PostRequest
    {
        public string Title { get; set; } = null !;
        public string Description { get; set; } = null !;
        public IFormFile Image { get; set; } = null !;
    }
}
